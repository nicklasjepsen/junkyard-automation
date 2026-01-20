using Godot;

namespace JunkyardAutomation;

/// <summary>
/// DebugOverlay - Displays debug information.
/// </summary>
public partial class DebugOverlay : Control
{
    private Label? _fpsLabel;
    private Label? _tickLabel;
    private Label? _gridPosLabel;

    private GridSystem? _gridSystem;
    private SimulationManager? _simulationManager;
    private TileHighlighter? _tileHighlighter;

    public override void _Ready()
    {
        _gridSystem = GetNode<GridSystem>("/root/GridSystem");
        _simulationManager = GetNode<SimulationManager>("/root/SimulationManager");

        SetupLabels();
    }

    /// <summary>
    /// Set reference to tile highlighter for grid position display.
    /// </summary>
    public void SetTileHighlighter(TileHighlighter highlighter)
    {
        _tileHighlighter = highlighter;
    }

    private void SetupLabels()
    {
        var container = new VBoxContainer();
        container.Position = new Vector2(10, 10);
        AddChild(container);

        _fpsLabel = new Label();
        _fpsLabel.Text = "FPS: --";
        container.AddChild(_fpsLabel);

        _tickLabel = new Label();
        _tickLabel.Text = "Tick: 0";
        container.AddChild(_tickLabel);

        _gridPosLabel = new Label();
        _gridPosLabel.Text = "Grid: (-, -)";
        container.AddChild(_gridPosLabel);
    }

    public override void _Process(double delta)
    {
        // Update FPS
        if (_fpsLabel != null)
        {
            _fpsLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}";
        }

        // Update tick count
        if (_tickLabel != null && _simulationManager != null)
        {
            var pauseStatus = _simulationManager.IsPaused ? " (PAUSED)" : "";
            _tickLabel.Text = $"Tick: {_simulationManager.CurrentTick}{pauseStatus}";
        }

        // Update grid position
        if (_gridPosLabel != null && _tileHighlighter != null)
        {
            var pos = _tileHighlighter.CurrentGridPos;
            _gridPosLabel.Text = $"Grid: ({pos.X}, {pos.Y})";
        }
    }
}
