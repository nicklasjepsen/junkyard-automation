using Godot;

namespace JunkyardAutomation;

/// <summary>
/// TileHighlighter - Visual feedback for mouse hover over tiles.
/// Draws a highlight on the tile under the mouse cursor.
/// </summary>
public partial class TileHighlighter : Node2D
{
    /// <summary>Default highlight color</summary>
    [Export]
    public Color DefaultColor { get; set; } = new Color(1, 1, 1, 0.3f);

    /// <summary>Valid placement color</summary>
    [Export]
    public Color ValidColor { get; set; } = new Color(0, 1, 0, 0.4f);

    /// <summary>Invalid placement color</summary>
    [Export]
    public Color InvalidColor { get; set; } = new Color(1, 0, 0, 0.4f);

    /// <summary>Current grid position under cursor</summary>
    public Vector2I CurrentGridPos { get; private set; } = new Vector2I(-1, -1);

    private Color _currentColor = Colors.Transparent;
    private bool _isVisibleHighlight = true;
    private Camera2D? _camera;
    private GridSystem? _gridSystem;

    public override void _Ready()
    {
        _gridSystem = GetNode<GridSystem>("/root/GridSystem");

        // Find camera after tree is ready
        CallDeferred(MethodName.FindCamera);
    }

    private void FindCamera()
    {
        _camera = GetViewport().GetCamera2D();
    }

    public override void _Process(double delta)
    {
        if (_camera == null || _gridSystem == null) return;

        var newPos = _gridSystem.ScreenToGrid(GetViewport().GetMousePosition(), _camera);

        if (newPos != CurrentGridPos)
        {
            CurrentGridPos = newPos;
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        if (!_isVisibleHighlight || _gridSystem == null) return;
        if (!_gridSystem.IsValidPosition(CurrentGridPos)) return;

        var corners = _gridSystem.GetTileCorners(CurrentGridPos);
        var color = _currentColor.A > 0 ? _currentColor : DefaultColor;

        // Draw filled diamond
        DrawColoredPolygon(corners, color);

        // Draw outline
        var outlineColor = new Color(color.R, color.G, color.B, Mathf.Min(color.A + 0.3f, 1.0f));
        for (int i = 0; i < 4; i++)
        {
            DrawLine(corners[i], corners[(i + 1) % 4], outlineColor, 2.0f);
        }
    }

    /// <summary>
    /// Set highlight to valid placement color.
    /// </summary>
    public void SetValid()
    {
        _currentColor = ValidColor;
        QueueRedraw();
    }

    /// <summary>
    /// Set highlight to invalid placement color.
    /// </summary>
    public void SetInvalid()
    {
        _currentColor = InvalidColor;
        QueueRedraw();
    }

    /// <summary>
    /// Reset to default color.
    /// </summary>
    public void SetDefault()
    {
        _currentColor = DefaultColor;
        QueueRedraw();
    }

    /// <summary>
    /// Hide the highlighter.
    /// </summary>
    public void HideHighlight()
    {
        _isVisibleHighlight = false;
        QueueRedraw();
    }

    /// <summary>
    /// Show the highlighter.
    /// </summary>
    public void ShowHighlight()
    {
        _isVisibleHighlight = true;
        QueueRedraw();
    }
}
