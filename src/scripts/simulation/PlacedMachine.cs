using Godot;
using System.Collections.Generic;

namespace JunkyardAutomation;

/// <summary>
/// Machine state enumeration.
/// </summary>
public enum MachineState
{
    Idle,
    Running,
    Blocked,
    Stalled
}

/// <summary>
/// PlacedMachine - Runtime instance of a machine in the simulation.
/// </summary>
public class PlacedMachine
{
    /// <summary>Unique instance ID</summary>
    public string Id { get; set; } = "";

    /// <summary>Reference to MachineDefinition type</summary>
    public string TypeId { get; set; } = "";

    /// <summary>Grid position (top-left corner for multi-tile machines)</summary>
    public Vector2I GridPosition { get; set; }

    /// <summary>Rotation in degrees (0, 90, 180, 270)</summary>
    public int Rotation { get; set; }

    /// <summary>Current machine state</summary>
    public MachineState State { get; set; } = MachineState.Idle;

    /// <summary>Items waiting to be processed</summary>
    public List<ItemEntity> InputBuffer { get; } = new();

    /// <summary>Processed items waiting to be output</summary>
    public List<ItemEntity> OutputBuffer { get; } = new();

    /// <summary>Current processing progress (0 to processing time)</summary>
    public float ProcessingTimer { get; set; }

    /// <summary>Machine-specific configuration (e.g., splitter filters)</summary>
    public Dictionary<string, object> Config { get; } = new();

    /// <summary>Reference to visual node (if any)</summary>
    public Node2D? Visual { get; set; }

    /// <summary>
    /// Get the output direction based on rotation.
    /// </summary>
    public int GetOutputDirection()
    {
        return Rotation;
    }

    /// <summary>
    /// Get the input direction (opposite of output).
    /// </summary>
    public int GetInputDirection()
    {
        return (Rotation + 180) % 360;
    }

    /// <summary>
    /// Serialize to dictionary for save/load.
    /// </summary>
    public Dictionary<string, object> ToDict()
    {
        var inputBufferData = new List<Dictionary<string, object>>();
        foreach (var item in InputBuffer)
        {
            inputBufferData.Add(item.ToDict());
        }

        var outputBufferData = new List<Dictionary<string, object>>();
        foreach (var item in OutputBuffer)
        {
            outputBufferData.Add(item.ToDict());
        }

        return new Dictionary<string, object>
        {
            ["id"] = Id,
            ["type_id"] = TypeId,
            ["grid_x"] = GridPosition.X,
            ["grid_y"] = GridPosition.Y,
            ["rotation"] = Rotation,
            ["state"] = (int)State,
            ["processing_timer"] = ProcessingTimer,
            ["input_buffer"] = inputBufferData,
            ["output_buffer"] = outputBufferData,
            ["config"] = Config
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
        Rotation = Convert.ToInt32(data.GetValueOrDefault("rotation", 0));
        State = (MachineState)Convert.ToInt32(data.GetValueOrDefault("state", 0));
        ProcessingTimer = Convert.ToSingle(data.GetValueOrDefault("processing_timer", 0f));

        // Note: input/output buffers and config would need additional deserialization logic
    }
}
