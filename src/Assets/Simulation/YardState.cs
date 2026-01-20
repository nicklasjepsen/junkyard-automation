using System;
using System.Collections.Generic;
using UnityEngine;

namespace JunkyardAutomation.Simulation
{
    /// <summary>
    /// Holds the complete state of the yard - all placed machines, items, etc.
    /// This is the source of truth for the simulation.
    /// </summary>
    public class YardState
    {
        public TileOccupancy Occupancy { get; private set; } = new TileOccupancy();
        public Dictionary<string, PlacedMachine> Machines { get; private set; } = new Dictionary<string, PlacedMachine>();
        public List<ItemEntity> Items { get; private set; } = new List<ItemEntity>();
        private Dictionary<string, ItemEntity> itemsById = new Dictionary<string, ItemEntity>();

        public event Action<PlacedMachine> OnMachinePlaced;
        public event Action<PlacedMachine> OnMachineRemoved;
        public event Action<ItemEntity> OnItemAdded;
        public event Action<ItemEntity> OnItemRemoved;

        /// <summary>
        /// Place a new machine in the yard.
        /// </summary>
        public PlacedMachine PlaceMachine(string machineTypeId, Vector2Int position, int rotation, Vector2Int size)
        {
            // Validate placement
            if (!Occupancy.CanPlace(position, size))
            {
                Debug.LogWarning($"[YardState] Cannot place {machineTypeId} at {position} - tiles occupied");
                return null;
            }

            // Create machine
            var machine = new PlacedMachine
            {
                Id = Guid.NewGuid().ToString(),
                MachineTypeId = machineTypeId,
                Position = position,
                Rotation = rotation
            };

            // Update state
            Machines[machine.Id] = machine;
            Occupancy.OccupyArea(position, size, machine.Id);

            Debug.Log($"[YardState] Placed {machineTypeId} at {position}, rotation {rotation}°, id={machine.Id}");

            OnMachinePlaced?.Invoke(machine);
            return machine;
        }

        /// <summary>
        /// Remove a machine from the yard.
        /// </summary>
        public bool RemoveMachine(string machineId, Vector2Int size)
        {
            if (!Machines.TryGetValue(machineId, out var machine))
            {
                Debug.LogWarning($"[YardState] Machine {machineId} not found");
                return false;
            }

            // Clear occupancy
            Occupancy.ClearArea(machine.Position, size);

            // Remove from state
            Machines.Remove(machineId);

            Debug.Log($"[YardState] Removed {machine.MachineTypeId} at {machine.Position}, id={machineId}");

            OnMachineRemoved?.Invoke(machine);
            return true;
        }

        /// <summary>
        /// Get machine at a specific tile, or null if empty.
        /// </summary>
        public PlacedMachine GetMachineAt(Vector2Int position)
        {
            string machineId = Occupancy.GetMachineAt(position);
            if (machineId == null) return null;

            return Machines.TryGetValue(machineId, out var machine) ? machine : null;
        }

        /// <summary>
        /// Check if a machine can be placed at the given position.
        /// </summary>
        public bool CanPlaceAt(Vector2Int position, Vector2Int size)
        {
            return Occupancy.CanPlace(position, size);
        }

        /// <summary>
        /// Get count of placed machines.
        /// </summary>
        public int MachineCount => Machines.Count;

        /// <summary>
        /// Get count of items.
        /// </summary>
        public int ItemCount => Items.Count;

        #region Item Management

        /// <summary>
        /// Add a new item to the yard.
        /// </summary>
        public ItemEntity AddItem(string typeId, Vector2Int position, int direction)
        {
            var item = new ItemEntity(typeId, position, direction);
            Items.Add(item);
            itemsById[item.Id] = item;

            Debug.Log($"[YardState] Added item {typeId} at {position}");

            OnItemAdded?.Invoke(item);
            return item;
        }

        /// <summary>
        /// Remove an item from the yard.
        /// </summary>
        public bool RemoveItem(string itemId)
        {
            if (!itemsById.TryGetValue(itemId, out var item))
            {
                return false;
            }

            Items.Remove(item);
            itemsById.Remove(itemId);

            OnItemRemoved?.Invoke(item);
            return true;
        }

        /// <summary>
        /// Get item by ID.
        /// </summary>
        public ItemEntity GetItem(string itemId)
        {
            return itemsById.TryGetValue(itemId, out var item) ? item : null;
        }

        /// <summary>
        /// Get all items at a specific tile.
        /// </summary>
        public IEnumerable<ItemEntity> GetItemsAt(Vector2Int position)
        {
            foreach (var item in Items)
            {
                if (item.Position == position)
                {
                    yield return item;
                }
            }
        }

        #endregion
    }
}
