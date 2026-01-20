# Feature Spec: Isometric Grid Foundation

## Goal
Establish the core rendering and input foundation that all other systems depend on. Players need to see an isometric grid, navigate it with camera controls, and identify which tile they're pointing at.

## Non-goals
- Tile placement/building (PR2)
- Machine rendering (PR2+)
- Item entities or movement (PR3+)
- Save/load functionality

## Design references
- ENGINE/ARCHITECTURE.md - GridSystem class
- ENGINE/TECH_STACK.md - Unity setup, 2:1 isometric ratio
- DESIGN/UI_UX.md - Placement UX (foundation for highlighting)
- ENGINE/PERFORMANCE.md - Debug overlay requirements

## User stories
- As a player, I can see an isometric grid representing my yard
- As a player, I can pan the camera by dragging or using WASD/arrow keys
- As a player, I can zoom in/out with mouse wheel
- As a player, I can see which tile my mouse is hovering over (highlight)
- As a developer, I can see debug info showing grid coordinates and FPS

## Behavior

### Inputs:
- Mouse position (screen space)
- Mouse wheel (zoom)
- Mouse drag (middle button) or WASD/arrows (pan)
- ESC (future: menu, currently no-op)

### Outputs:
- Rendered isometric grid (placeholder tiles)
- Highlighted tile under mouse cursor
- Debug overlay with coordinates

### State changes:
- Camera position (pan)
- Camera orthographic size (zoom)
- Currently hovered tile coordinate

### Failure cases:
- Mouse outside grid bounds → no highlight shown
- Zoom at min/max limits → clamp, no further zoom

## Data

### Grid Configuration (hardcoded for M0, data-driven later):
```csharp
// src/Assets/Core/GridSystem.cs
public class GridConfig
{
    public int GridWidth = 32;          // tiles
    public int GridHeight = 32;         // tiles
    public float TileWidth = 1.0f;      // world units
    public float TileHeight = 0.5f;     // world units (2:1 ratio)
}
```

### Camera Configuration:
```csharp
// src/Assets/Core/CameraController.cs
public class CameraConfig
{
    public float MinZoom = 3f;          // orthographic size
    public float MaxZoom = 15f;
    public float ZoomSpeed = 2f;
    public float PanSpeed = 10f;
}
```

## UI/UX

### Screens affected:
- Main game scene (new)

### Debug overlay (top-left corner):
- "Grid: (X, Y)" - hovered tile coordinates
- "World: (X.XX, Y.YY)" - world position
- "FPS: XX"
- Toggle with F3 key

### Visual elements:
- Grid floor tiles (placeholder white/gray checkerboard)
- Tile highlight (semi-transparent colored overlay on hovered tile)

## Acceptance criteria
- [ ] Unity project opens without errors
- [ ] Isometric grid renders with visible tile boundaries
- [ ] Camera pans with WASD/arrows and middle-mouse drag
- [ ] Camera zooms with mouse wheel, respects min/max limits
- [ ] Mouse hover highlights correct tile
- [ ] Highlight disappears when mouse is outside grid
- [ ] Debug overlay shows coordinates and FPS
- [ ] Debug overlay toggles with F3
- [ ] Coordinate conversion is accurate (click tile center = that tile's coords)
- [ ] Performance: 60 FPS with 32x32 grid

## Test plan

### Manual:
1. Open project in Unity, enter Play mode - grid should render
2. Press WASD - camera should pan smoothly
3. Scroll mouse wheel - camera should zoom in/out
4. Move mouse over grid - tiles should highlight one at a time
5. Move mouse outside grid bounds - no highlight
6. Press F3 - debug overlay toggles
7. Verify coordinates: hover tile (0,0) should show "Grid: (0, 0)"
8. Verify coordinates: hover tile (5,10) should show "Grid: (5, 10)"
9. Zoom to min/max - should stop at limits

### Automated (future):
- Unit tests for coordinate conversion functions
- `WorldToGrid(GridToWorld(pos)) == pos` for all valid positions
