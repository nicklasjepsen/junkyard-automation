using Godot;
using System.Collections.Generic;

namespace JunkyardAutomation;

/// <summary>
/// YardState - Current state of the yard (serializable for save/load).
/// Contains all runtime state for the simulation.
/// </summary>
public class YardState
{
    /// <summary>All placed machines by ID</summary>
    public Dictionary<string, PlacedMachine> Machines { get; } = new();

    /// <summary>All active items by ID</summary>
    public Dictionary<string, ItemEntity> Items { get; } = new();

    /// <summary>Tile occupancy tracking (pos key -> occupant info)</summary>
    private readonly Dictionary<string, OccupantInfo> _tileOccupancy = new();

    private int _nextMachineId = 1;
    private int _nextItemId = 1;

    private GridSystem? _gridSystem;
    private ContentRegistry? _contentRegistry;

    /// <summary>
    /// Initialize with autoload references.
    /// </summary>
    public void Initialize(GridSystem gridSystem, ContentRegistry contentRegistry)
    {
        _gridSystem = gridSystem;
        _contentRegistry = contentRegistry;
    }

    /// <summary>
    /// Generate a unique machine instance ID.
    /// </summary>
    public string GenerateMachineId()
    {
        return $"machine_{_nextMachineId++}";
    }

    /// <summary>
    /// Generate a unique item instance ID.
    /// </summary>
    public string GenerateItemId()
    {
        return $"item_{_nextItemId++}";
    }

    /// <summary>
    /// Add a machine to the yard.
    /// </summary>
    public void AddMachine(PlacedMachine machine)
    {
        Machines[machine.Id] = machine;
        UpdateOccupancyForMachine(machine, true);
    }

    /// <summary>
    /// Remove a machine from the yard.
    /// </summary>
    public void RemoveMachine(string machineId)
    {
        if (Machines.TryGetValue(machineId, out var machine))
        {
            UpdateOccupancyForMachine(machine, false);
            Machines.Remove(machineId);
        }
    }

    /// <summary>
    /// Add an item to the yard.
    /// </summary>
    public void AddItem(ItemEntity item)
    {
        Items[item.Id] = item;
    }

    /// <summary>
    /// Remove an item from the yard.
    /// </summary>
    public void RemoveItem(string itemId)
    {
        Items.Remove(itemId);
    }

    /// <summary>
    /// Get machine at a grid position.
    /// </summary>
    public PlacedMachine? GetMachineAt(Vector2I gridPos)
    {
        var key = PosToKey(gridPos);
        if (_tileOccupancy.TryGetValue(key, out var occupant))
        {
            return Machines.GetValueOrDefault(occupant.MachineId);
        }
        return null;
    }

    /// <summary>
    /// Check if a tile is occupied.
    /// </summary>
    public bool IsTileOccupied(Vector2I gridPos)
    {
        return _tileOccupancy.ContainsKey(PosToKey(gridPos));
    }

    /// <summary>
    /// Check if a machine can be placed at position (considering its size).
    /// </summary>
    public bool CanPlaceMachineAt(Vector2I gridPos, string machineTypeId)
    {
        if (_contentRegistry == null || _gridSystem == null) return false;

        var machineDef = _contentRegistry.GetMachine(machineTypeId);
        if (machineDef == null) return false;

        var size = GetMachineSize(machineDef);

        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
            {
                var checkPos = gridPos + new Vector2I(x, y);
                if (!_gridSystem.IsValidPosition(checkPos)) return false;
                if (IsTileOccupied(checkPos)) return false;
            }
        }
        return true;
    }

    private void UpdateOccupancyForMachine(PlacedMachine machine, bool isAdding)
    {
        if (_contentRegistry == null) return;

        var machineDef = _contentRegistry.GetMachine(machine.TypeId);
        if (machineDef == null) return;

        var size = GetMachineSize(machineDef);

        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
            {
                var tilePos = machine.GridPosition + new Vector2I(x, y);
                var key = PosToKey(tilePos);

                if (isAdding)
                {
                    _tileOccupancy[key] = new OccupantInfo { MachineId = machine.Id };
                }
                else
                {
                    _tileOccupancy.Remove(key);
                }
            }
        }
    }

    private static Vector2I GetMachineSize(Dictionary<string, Variant> machineDef)
    {
        if (machineDef.TryGetValue("size", out var sizeVar))
        {
            if (sizeVar.VariantType == Variant.Type.Array)
            {
                var arr = sizeVar.AsInt32Array();
                if (arr.Length >= 2)
                {
                    return new Vector2I(arr[0], arr[1]);
                }
            }
        }
        return new Vector2I(1, 1);
    }

    private static string PosToKey(Vector2I pos)
    {
        return $"{pos.X},{pos.Y}";
    }

    /// <summary>
    /// Clear all state.
    /// </summary>
    public void Clear()
    {
        Machines.Clear();
        Items.Clear();
        _tileOccupancy.Clear();
        _nextMachineId = 1;
        _nextItemId = 1;
    }

    private class OccupantInfo
    {
        public string MachineId { get; set; } = "";
    }
}
