using Godot;

namespace JunkyardAutomation;

/// <summary>
/// GridRenderer - Procedural isometric grid drawing.
/// Uses Godot's _Draw() for efficient rendering.
/// </summary>
public partial class GridRenderer : Node2D
{
    /// <summary>Grid line color</summary>
    [Export]
    public Color LineColor { get; set; } = new Color(0.3f, 0.3f, 0.3f, 0.5f);

    /// <summary>Grid line width</summary>
    [Export]
    public float LineWidth { get; set; } = 1.0f;

    /// <summary>Whether to draw the grid</summary>
    [Export]
    public bool VisibleGrid { get; set; } = true;

    private GridSystem? _gridSystem;

    public override void _Ready()
    {
        _gridSystem = GetNode<GridSystem>("/root/GridSystem");

        // Redraw when grid changes
        _gridSystem.GridSizeChanged += OnGridSizeChanged;
    }

    public override void _Draw()
    {
        if (!VisibleGrid || _gridSystem == null) return;
        DrawGridLines();
    }

    private void DrawGridLines()
    {
        if (_gridSystem == null) return;

        int gridW = _gridSystem.GridWidth;
        int gridH = _gridSystem.GridHeight;

        // Draw vertical lines (along x axis in grid space)
        for (int x = 0; x <= gridW; x++)
        {
            var start = _gridSystem.GridToWorld(new Vector2I(x, 0));
            var end = _gridSystem.GridToWorld(new Vector2I(x, gridH));
            DrawLine(start, end, LineColor, LineWidth);
        }

        // Draw horizontal lines (along y axis in grid space)
        for (int y = 0; y <= gridH; y++)
        {
            var start = _gridSystem.GridToWorld(new Vector2I(0, y));
            var end = _gridSystem.GridToWorld(new Vector2I(gridW, y));
            DrawLine(start, end, LineColor, LineWidth);
        }
    }

    private void OnGridSizeChanged(int width, int height)
    {
        QueueRedraw();
    }

    /// <summary>
    /// Toggle grid visibility.
    /// </summary>
    public void ToggleVisibility()
    {
        VisibleGrid = !VisibleGrid;
        QueueRedraw();
    }

    /// <summary>
    /// Set grid line color.
    /// </summary>
    public void SetLineColor(Color color)
    {
        LineColor = color;
        QueueRedraw();
    }
}
