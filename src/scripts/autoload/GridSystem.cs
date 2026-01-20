using Godot;

namespace JunkyardAutomation;

/// <summary>
/// GridSystem - Isometric coordinate conversion and grid management.
/// Handles all coordinate conversions between grid, world, and screen space.
/// </summary>
public partial class GridSystem : Node
{
    [Signal]
    public delegate void GridSizeChangedEventHandler(int newWidth, int newHeight);

    /// <summary>Tile width in pixels (2:1 isometric ratio)</summary>
    public int TileWidth { get; set; } = 64;

    /// <summary>Tile height in pixels (2:1 isometric ratio)</summary>
    public int TileHeight { get; set; } = 32;

    /// <summary>Grid width in tiles</summary>
    public int GridWidth { get; set; } = 32;

    /// <summary>Grid height in tiles</summary>
    public int GridHeight { get; set; } = 32;

    /// <summary>
    /// Convert grid coordinates to world position (center of tile).
    /// </summary>
    public Vector2 GridToWorld(Vector2I gridPos)
    {
        float x = (gridPos.X - gridPos.Y) * TileWidth / 2.0f;
        float y = (gridPos.X + gridPos.Y) * TileHeight / 2.0f;
        return new Vector2(x, y);
    }

    /// <summary>
    /// Convert world position to grid coordinates.
    /// </summary>
    public Vector2I WorldToGrid(Vector2 worldPos)
    {
        float x = (worldPos.X / (TileWidth / 2.0f) + worldPos.Y / (TileHeight / 2.0f)) / 2.0f;
        float y = (worldPos.Y / (TileHeight / 2.0f) - worldPos.X / (TileWidth / 2.0f)) / 2.0f;
        return new Vector2I(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }

    /// <summary>
    /// Convert screen position to grid coordinates (accounts for camera).
    /// </summary>
    public Vector2I ScreenToGrid(Vector2 screenPos, Camera2D camera)
    {
        var worldPos = camera.GetGlobalMousePosition();
        return WorldToGrid(worldPos);
    }

    /// <summary>
    /// Check if grid position is within bounds.
    /// </summary>
    public bool IsValidPosition(Vector2I gridPos)
    {
        return gridPos.X >= 0 && gridPos.X < GridWidth &&
               gridPos.Y >= 0 && gridPos.Y < GridHeight;
    }

    /// <summary>
    /// Get the corner points of a tile in world coordinates (for drawing).
    /// Returns points in order: Top, Right, Bottom, Left.
    /// </summary>
    public Vector2[] GetTileCorners(Vector2I gridPos)
    {
        var center = GridToWorld(gridPos);
        float halfW = TileWidth / 2.0f;
        float halfH = TileHeight / 2.0f;

        return new Vector2[]
        {
            center + new Vector2(0, -halfH),    // Top
            center + new Vector2(halfW, 0),     // Right
            center + new Vector2(0, halfH),     // Bottom
            center + new Vector2(-halfW, 0),    // Left
        };
    }

    /// <summary>
    /// Get neighboring grid positions (4-directional).
    /// </summary>
    public Vector2I[] GetNeighbors(Vector2I gridPos)
    {
        var offsets = new Vector2I[]
        {
            new(1, 0), new(-1, 0), new(0, 1), new(0, -1)
        };

        var neighbors = new System.Collections.Generic.List<Vector2I>();
        foreach (var offset in offsets)
        {
            var neighbor = gridPos + offset;
            if (IsValidPosition(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors.ToArray();
    }

    /// <summary>
    /// Get the offset vector for a direction (in degrees).
    /// </summary>
    public Vector2I DirectionToOffset(int direction)
    {
        return direction switch
        {
            0 => new Vector2I(0, -1),     // North
            90 => new Vector2I(1, 0),     // East
            180 => new Vector2I(0, 1),    // South
            270 => new Vector2I(-1, 0),   // West
            _ => Vector2I.Zero
        };
    }

    /// <summary>
    /// Get the opposite direction.
    /// </summary>
    public int OppositeDirection(int direction)
    {
        return (direction + 180) % 360;
    }
}
