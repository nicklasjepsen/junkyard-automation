using Godot;
using System.Collections.Generic;

namespace JunkyardAutomation;

/// <summary>
/// ConveyorSystem - Moves items along conveyor belts.
/// </summary>
public partial class ConveyorSystem : Node
{
    /// <summary>Speed of item movement (progress per tick)</summary>
    [Export]
    public float MoveSpeed { get; set; } = 0.2f;

    private YardState? _yardState;
    private GridSystem? _gridSystem;

    public void Initialize(YardState yardState, GridSystem gridSystem)
    {
        _yardState = yardState;
        _gridSystem = gridSystem;
    }

    /// <summary>
    /// Process one simulation tick.
    /// </summary>
    public void Tick()
    {
        if (_yardState == null || _gridSystem == null) return;

        // Get all items on conveyors
        var itemsToProcess = new List<ItemEntity>(_yardState.Items.Values);

        foreach (var item in itemsToProcess)
        {
            ProcessItem(item);
        }
    }

    private void ProcessItem(ItemEntity item)
    {
        if (_yardState == null || _gridSystem == null) return;

        // Check if item is on a conveyor
        var machine = _yardState.GetMachineAt(item.GridPosition);
        if (machine == null || machine.TypeId != "conveyor") return;

        // Move item forward
        item.TileProgress += MoveSpeed;

        // If item has crossed the tile boundary
        if (item.TileProgress >= 1.0f)
        {
            item.TileProgress -= 1.0f;

            // Calculate next position based on conveyor direction
            var offset = _gridSystem.DirectionToOffset(machine.Rotation);
            var nextPos = item.GridPosition + offset;

            // Check if next tile is valid and has a conveyor or machine
            if (_gridSystem.IsValidPosition(nextPos))
            {
                var nextMachine = _yardState.GetMachineAt(nextPos);
                if (nextMachine != null)
                {
                    // Move to next tile
                    item.GridPosition = nextPos;
                    item.Direction = machine.Rotation;
                }
                else
                {
                    // No destination - item stops at edge
                    item.TileProgress = 1.0f;
                }
            }
            else
            {
                // Off grid - item stops at edge
                item.TileProgress = 1.0f;
            }
        }

        // Update visual position
        UpdateItemVisual(item);
    }

    private void UpdateItemVisual(ItemEntity item)
    {
        if (_gridSystem == null || item.Visual == null) return;

        // Calculate world position based on grid position and tile progress
        var basePos = _gridSystem.GridToWorld(item.GridPosition);
        var offset = _gridSystem.DirectionToOffset(item.Direction);
        var progressOffset = new Vector2(offset.X, offset.Y) * item.TileProgress *
                            new Vector2(_gridSystem.TileWidth / 2.0f, _gridSystem.TileHeight / 2.0f);

        item.Visual.Position = basePos + progressOffset;
    }
}
