using Godot;
using System.Collections.Generic;

namespace JunkyardAutomation;

/// <summary>
/// ItemEntity - Runtime instance of an item in the simulation.
/// </summary>
public class ItemEntity
{
    /// <summary>Unique instance ID</summary>
    public string Id { get; set; } = "";

    /// <summary>Reference to ItemDefinition type</summary>
    public string TypeId { get; set; } = "";

    /// <summary>Current grid position</summary>
    public Vector2I GridPosition { get; set; }

    /// <summary>Progress across current tile (0-1)</summary>
    public float TileProgress { get; set; }

    /// <summary>Movement direction (0, 90, 180, 270)</summary>
    public int Direction { get; set; }

    /// <summary>Reference to visual node (if any)</summary>
    public Node2D? Visual { get; set; }

    /// <summary>
    /// Serialize to dictionary for save/load.
    /// </summary>
    public Dictionary<string, object> ToDict()
    {
        return new Dictionary<string, object>
        {
            ["id"] = Id,
            ["type_id"] = TypeId,
            ["grid_x"] = GridPosition.X,
            ["grid_y"] = GridPosition.Y,
            ["tile_progress"] = TileProgress,
            ["direction"] = Direction
        };
    }

    /// <summary>
    /// Deserialize from dictionary.
    /// </summary>
    public void FromDict(Dictionary<string, object> data)
    {
        Id = data.GetValueOrDefault("id", "")?.ToString() ?? "";
        TypeId = data.GetValueOrDefault("type_id", "")?.ToString() ?? "";
        GridPosition = new Vector2I(
            Convert.ToInt32(data.GetValueOrDefault("grid_x", 0)),
            Convert.ToInt32(data.GetValueOrDefault("grid_y", 0))
        );
        TileProgress = Convert.ToSingle(data.GetValueOrDefault("tile_progress", 0f));
        Direction = Convert.ToInt32(data.GetValueOrDefault("direction", 0));
    }
}
