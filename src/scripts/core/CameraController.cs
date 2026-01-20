using Godot;

namespace JunkyardAutomation;

/// <summary>
/// CameraController - Pan and zoom controls for isometric view.
/// Supports WASD panning and mouse wheel zoom.
/// </summary>
public partial class CameraController : Camera2D
{
    /// <summary>Pan speed in pixels per second</summary>
    [Export]
    public float PanSpeed { get; set; } = 500.0f;

    /// <summary>Minimum zoom level</summary>
    [Export]
    public float ZoomMin { get; set; } = 0.5f;

    /// <summary>Maximum zoom level</summary>
    [Export]
    public float ZoomMax { get; set; } = 2.0f;

    /// <summary>Zoom speed per scroll step</summary>
    [Export]
    public float ZoomSpeed { get; set; } = 0.1f;

    private float _currentZoom = 1.0f;
    private GridSystem? _gridSystem;

    public override void _Ready()
    {
        _gridSystem = GetNode<GridSystem>("/root/GridSystem");
        CenterOnGrid();
    }

    public override void _Process(double delta)
    {
        HandlePanInput((float)delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        HandleZoomInput(@event);
    }

    private void HandlePanInput(float delta)
    {
        var panDirection = Vector2.Zero;

        if (Input.IsActionPressed("camera_pan_up"))
            panDirection.Y -= 1;
        if (Input.IsActionPressed("camera_pan_down"))
            panDirection.Y += 1;
        if (Input.IsActionPressed("camera_pan_left"))
            panDirection.X -= 1;
        if (Input.IsActionPressed("camera_pan_right"))
            panDirection.X += 1;

        if (panDirection != Vector2.Zero)
        {
            panDirection = panDirection.Normalized();
            Position += panDirection * PanSpeed * delta / _currentZoom;
        }
    }

    private void HandleZoomInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
                ZoomIn();
            else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
                ZoomOut();
        }
    }

    private void ZoomIn()
    {
        _currentZoom = Mathf.Min(_currentZoom + ZoomSpeed, ZoomMax);
        Zoom = new Vector2(_currentZoom, _currentZoom);
    }

    private void ZoomOut()
    {
        _currentZoom = Mathf.Max(_currentZoom - ZoomSpeed, ZoomMin);
        Zoom = new Vector2(_currentZoom, _currentZoom);
    }

    private void CenterOnGrid()
    {
        if (_gridSystem == null) return;

        var centerGrid = new Vector2I(_gridSystem.GridWidth / 2, _gridSystem.GridHeight / 2);
        Position = _gridSystem.GridToWorld(centerGrid);
    }

    /// <summary>
    /// Set zoom level directly.
    /// </summary>
    public void SetZoomLevel(float level)
    {
        _currentZoom = Mathf.Clamp(level, ZoomMin, ZoomMax);
        Zoom = new Vector2(_currentZoom, _currentZoom);
    }

    /// <summary>
    /// Pan to a specific grid position.
    /// </summary>
    public void PanToGrid(Vector2I gridPos)
    {
        if (_gridSystem != null)
        {
            Position = _gridSystem.GridToWorld(gridPos);
        }
    }
}
