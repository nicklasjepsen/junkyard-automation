using Godot;

namespace JunkyardAutomation;

/// <summary>
/// Placement mode enumeration.
/// </summary>
public enum PlacementMode
{
    None,
    Build,
    Demolish
}

/// <summary>
/// PlacementManager - Handles building and demolishing machines.
/// </summary>
public partial class PlacementManager : Node
{
    [Signal]
    public delegate void ModeChangedEventHandler(PlacementMode mode);

    [Signal]
    public delegate void MachinePlacedEventHandler(PlacedMachine machine);

    [Signal]
    public delegate void MachineDemolishedEventHandler(string machineId);

    /// <summary>Current placement mode</summary>
    public PlacementMode CurrentMode { get; private set; } = PlacementMode.None;

    /// <summary>Currently selected machine type for building</summary>
    public string? SelectedMachineType { get; private set; }

    /// <summary>Current rotation for placement (0, 90, 180, 270)</summary>
    public int CurrentRotation { get; private set; } = 0;

    private YardState? _yardState;
    private GridSystem? _gridSystem;
    private ContentRegistry? _contentRegistry;
    private TileHighlighter? _tileHighlighter;
    private PlacementGhost? _placementGhost;
    private Node2D? _machinesContainer;

    public override void _Ready()
    {
        _gridSystem = GetNode<GridSystem>("/root/GridSystem");
        _contentRegistry = GetNode<ContentRegistry>("/root/ContentRegistry");
    }

    /// <summary>
    /// Initialize with required references.
    /// </summary>
    public void Initialize(YardState yardState, TileHighlighter tileHighlighter,
                          PlacementGhost placementGhost, Node2D machinesContainer)
    {
        _yardState = yardState;
        _tileHighlighter = tileHighlighter;
        _placementGhost = placementGhost;
        _machinesContainer = machinesContainer;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("rotate_cw"))
        {
            RotateClockwise();
        }
        else if (@event.IsActionPressed("cancel"))
        {
            CancelPlacement();
        }
        else if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                HandleLeftClick();
            }
        }
    }

    public override void _Process(double delta)
    {
        if (CurrentMode == PlacementMode.Build)
        {
            UpdateBuildPreview();
        }
        else if (CurrentMode == PlacementMode.Demolish)
        {
            UpdateDemolishPreview();
        }
    }

    /// <summary>
    /// Enter build mode with the specified machine type.
    /// </summary>
    public void EnterBuildMode(string machineTypeId)
    {
        SelectedMachineType = machineTypeId;
        CurrentMode = PlacementMode.Build;
        CurrentRotation = 0;

        _placementGhost?.Show();
        _placementGhost?.SetMachineType(machineTypeId);

        EmitSignal(SignalName.ModeChanged, (int)CurrentMode);
    }

    /// <summary>
    /// Enter demolish mode.
    /// </summary>
    public void EnterDemolishMode()
    {
        SelectedMachineType = null;
        CurrentMode = PlacementMode.Demolish;

        _placementGhost?.Hide();
        _tileHighlighter?.SetInvalid();

        EmitSignal(SignalName.ModeChanged, (int)CurrentMode);
    }

    /// <summary>
    /// Cancel current placement operation.
    /// </summary>
    public void CancelPlacement()
    {
        CurrentMode = PlacementMode.None;
        SelectedMachineType = null;

        _placementGhost?.Hide();
        _tileHighlighter?.SetDefault();

        EmitSignal(SignalName.ModeChanged, (int)CurrentMode);
    }

    /// <summary>
    /// Rotate placement clockwise by 90 degrees.
    /// </summary>
    public void RotateClockwise()
    {
        CurrentRotation = (CurrentRotation + 90) % 360;
        _placementGhost?.SetRotation(CurrentRotation);
    }

    private void HandleLeftClick()
    {
        if (_tileHighlighter == null) return;

        var gridPos = _tileHighlighter.CurrentGridPos;

        if (CurrentMode == PlacementMode.Build)
        {
            TryPlaceMachine(gridPos);
        }
        else if (CurrentMode == PlacementMode.Demolish)
        {
            TryDemolishMachine(gridPos);
        }
    }

    private void TryPlaceMachine(Vector2I gridPos)
    {
        if (_yardState == null || _gridSystem == null || _contentRegistry == null)
            return;

        if (string.IsNullOrEmpty(SelectedMachineType)) return;

        if (!_yardState.CanPlaceMachineAt(gridPos, SelectedMachineType))
        {
            GD.Print($"Cannot place {SelectedMachineType} at {gridPos}");
            return;
        }

        // Create the machine
        var machine = new PlacedMachine
        {
            Id = _yardState.GenerateMachineId(),
            TypeId = SelectedMachineType,
            GridPosition = gridPos,
            Rotation = CurrentRotation,
            State = MachineState.Idle
        };

        _yardState.AddMachine(machine);

        // Create visual
        CreateMachineVisual(machine);

        GD.Print($"Placed {machine.TypeId} at {gridPos} with rotation {CurrentRotation}");
        EmitSignal(SignalName.MachinePlaced, machine);
    }

    private void TryDemolishMachine(Vector2I gridPos)
    {
        if (_yardState == null) return;

        var machine = _yardState.GetMachineAt(gridPos);
        if (machine == null)
        {
            GD.Print($"No machine at {gridPos} to demolish");
            return;
        }

        // Remove visual
        machine.Visual?.QueueFree();

        var machineId = machine.Id;
        _yardState.RemoveMachine(machineId);

        GD.Print($"Demolished machine at {gridPos}");
        EmitSignal(SignalName.MachineDemolished, machineId);
    }

    private void CreateMachineVisual(PlacedMachine machine)
    {
        if (_gridSystem == null || _machinesContainer == null) return;

        var visual = new MachineVisual();
        visual.Initialize(machine, _gridSystem);
        _machinesContainer.AddChild(visual);
        machine.Visual = visual;
    }

    private void UpdateBuildPreview()
    {
        if (_tileHighlighter == null || _yardState == null || _placementGhost == null)
            return;

        if (string.IsNullOrEmpty(SelectedMachineType)) return;

        var gridPos = _tileHighlighter.CurrentGridPos;
        var canPlace = _yardState.CanPlaceMachineAt(gridPos, SelectedMachineType);

        if (canPlace)
        {
            _tileHighlighter.SetValid();
            _placementGhost.SetValid();
        }
        else
        {
            _tileHighlighter.SetInvalid();
            _placementGhost.SetInvalid();
        }

        _placementGhost.UpdatePosition(gridPos);
    }

    private void UpdateDemolishPreview()
    {
        if (_tileHighlighter == null || _yardState == null) return;

        var gridPos = _tileHighlighter.CurrentGridPos;
        var hasMachine = _yardState.GetMachineAt(gridPos) != null;

        if (hasMachine)
        {
            _tileHighlighter.SetInvalid(); // Red to indicate demolition target
        }
        else
        {
            _tileHighlighter.SetDefault();
        }
    }
}
