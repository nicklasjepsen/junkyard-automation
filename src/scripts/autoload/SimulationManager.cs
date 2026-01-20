using Godot;

namespace JunkyardAutomation;

/// <summary>
/// SimulationManager - Fixed timestep tick-based simulation coordinator.
/// Runs in _PhysicsProcess at a configurable tick rate (default 20 ticks/sec).
/// Coordinates all simulation systems in a deterministic order.
/// </summary>
public partial class SimulationManager : Node
{
    [Signal]
    public delegate void TickCompletedEventHandler(int tickNumber);

    [Signal]
    public delegate void SimulationPausedEventHandler();

    [Signal]
    public delegate void SimulationResumedEventHandler();

    /// <summary>Current simulation tick number</summary>
    public int CurrentTick { get; private set; } = 0;

    /// <summary>Whether simulation is paused</summary>
    public bool IsPaused { get; private set; } = true;

    // References to simulation systems (set during initialization)
    public Node? ConveyorSystem { get; set; }
    public Node? MachineSystem { get; set; }
    public Node? DeliverySystem { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        if (IsPaused) return;
        ExecuteTick();
    }

    private void ExecuteTick()
    {
        CurrentTick++;

        // Run systems in deterministic order:
        // 1. Delivery - Spawn new items at delivery dock
        if (DeliverySystem?.HasMethod("Tick") == true)
        {
            DeliverySystem.Call("Tick");
        }

        // 2. Conveyor - Move items along belts
        if (ConveyorSystem?.HasMethod("Tick") == true)
        {
            ConveyorSystem.Call("Tick");
        }

        // 3. Machine - Process items in machines
        if (MachineSystem?.HasMethod("Tick") == true)
        {
            MachineSystem.Call("Tick");
        }

        EmitSignal(SignalName.TickCompleted, CurrentTick);
    }

    /// <summary>
    /// Pause the simulation.
    /// </summary>
    public void Pause()
    {
        if (!IsPaused)
        {
            IsPaused = true;
            EmitSignal(SignalName.SimulationPaused);
        }
    }

    /// <summary>
    /// Resume the simulation.
    /// </summary>
    public void Resume()
    {
        if (IsPaused)
        {
            IsPaused = false;
            EmitSignal(SignalName.SimulationResumed);
        }
    }

    /// <summary>
    /// Toggle pause state.
    /// </summary>
    public void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    /// <summary>
    /// Reset the simulation to initial state.
    /// </summary>
    public void Reset()
    {
        CurrentTick = 0;
        IsPaused = true;
    }

    /// <summary>
    /// Register a system for tick updates.
    /// </summary>
    public void RegisterSystem(string systemName, Node systemNode)
    {
        switch (systemName.ToLower())
        {
            case "conveyor":
                ConveyorSystem = systemNode;
                break;
            case "machine":
                MachineSystem = systemNode;
                break;
            case "delivery":
                DeliverySystem = systemNode;
                break;
            default:
                GD.PushWarning($"Unknown system: {systemName}");
                break;
        }
    }
}
