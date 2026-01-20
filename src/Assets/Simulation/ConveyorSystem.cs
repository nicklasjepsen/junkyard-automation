using System.Collections.Generic;
using UnityEngine;
using JunkyardAutomation.Data;

namespace JunkyardAutomation.Simulation
{
    /// <summary>
    /// Handles item movement along conveyor belts.
    /// Items advance each tick and transfer between connected conveyors.
    /// </summary>
    public static class ConveyorSystem
    {
        private const int DEFAULT_TICKS_PER_MOVE = 4;

        /// <summary>
        /// Process one simulation tick for all items on conveyors.
        /// </summary>
        public static void Tick(YardState yard)
        {
            if (yard == null) return;

            var items = yard.Items;
            if (items == null || items.Count == 0) return;

            // Save previous states for interpolation
            foreach (var item in items)
            {
                item.SavePreviousState();
            }

            // Process items (iterate in reverse to allow removal)
            List<ItemEntity> toRemove = null;

            for (int i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];
                bool shouldRemove = ProcessItem(yard, item);

                if (shouldRemove)
                {
                    toRemove ??= new List<ItemEntity>();
                    toRemove.Add(item);
                }
            }

            // Remove items that reached dead ends
            if (toRemove != null)
            {
                foreach (var item in toRemove)
                {
                    yard.RemoveItem(item.Id);
                }
            }
        }

        /// <summary>
        /// Process a single item for one tick.
        /// Returns true if item should be removed.
        /// </summary>
        private static bool ProcessItem(YardState yard, ItemEntity item)
        {
            // Get the conveyor this item is on
            var conveyor = yard.GetMachineAt(item.Position);
            if (conveyor == null || conveyor.MachineTypeId != "Conveyor")
            {
                // Item is not on a conveyor, remove it
                return true;
            }

            // Get conveyor speed from definition
            int ticksPerMove = DEFAULT_TICKS_PER_MOVE;
            var def = ContentRegistry.GetMachine("Conveyor");
            if (def != null && def.ticksPerMove > 0)
            {
                ticksPerMove = def.ticksPerMove;
            }

            // Update item direction to match conveyor
            item.Direction = conveyor.Rotation;

            // Advance progress
            float progressPerTick = 1f / ticksPerMove;
            item.Progress += progressPerTick;

            // Check if ready to move to next tile
            if (item.Progress >= 1f)
            {
                Vector2Int nextTile = item.GetNextTile();

                // Check if next tile has a conveyor that can accept the item
                if (CanTransferTo(yard, item, nextTile))
                {
                    // Move to next tile
                    item.Position = nextTile;
                    item.Progress = 0f;
                }
                else
                {
                    // Can't move - wait at edge
                    item.Progress = 1f;

                    // Check if we should despawn (dead end)
                    var nextMachine = yard.GetMachineAt(nextTile);
                    if (nextMachine == null)
                    {
                        // Dead end - despawn item
                        Debug.Log($"[ConveyorSystem] Item {item.TypeId} despawned at dead end {item.Position}");
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Check if an item can transfer to the target tile.
        /// </summary>
        private static bool CanTransferTo(YardState yard, ItemEntity item, Vector2Int targetTile)
        {
            var targetMachine = yard.GetMachineAt(targetTile);
            if (targetMachine == null) return false;

            // For now, only conveyors can accept items
            if (targetMachine.MachineTypeId != "Conveyor") return false;

            // Check if target conveyor is facing a compatible direction
            // (not facing directly back at us)
            int itemDir = item.Direction;
            int targetDir = targetMachine.Rotation;

            // Opposite direction check (180 degrees different)
            int diff = Mathf.Abs(itemDir - targetDir);
            if (diff == 180)
            {
                // Conveyors facing each other - can't transfer
                return false;
            }

            // Check if another item is already at the entrance of target tile
            var itemsAtTarget = yard.GetItemsAt(targetTile);
            foreach (var existing in itemsAtTarget)
            {
                // If there's an item at progress < 0.5, the tile entrance is blocked
                if (existing.Progress < 0.5f)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
