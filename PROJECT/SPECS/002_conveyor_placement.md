# Feature Spec: Conveyor Placement System

## Goal
Allow players to place and remove conveyor belts on the isometric grid, establishing the foundation for all machine placement in the game.

## Non-goals
- Other machine types (splitters, processors) - future PRs
- Conveyor belt animation/item movement - future PR
- Save/load of placed machines - future PR
- Cost/money system - future PR

## Design references
- DESIGN/UI_UX.md - Placement UX (ghost preview, rotate, confirm/cancel)
- DESIGN/SYSTEMS_SORTING.md - Conveyor definition
- ENGINE/ARCHITECTURE.md - TileOccupancy, MachineEntity

## User stories
- As a player, I can select "Conveyor" from a build menu
- As a player, I can see a ghost preview of the conveyor following my mouse
- As a player, I can rotate the conveyor with R key (4 directions)
- As a player, I can place a conveyor with left-click on valid tiles
- As a player, I can see red highlight when placement is invalid (occupied tile)
- As a player, I can cancel placement with right-click or ESC
- As a player, I can demolish placed conveyors with a demolish tool

## Behavior

### Inputs:
- Left-click: Place machine / Demolish machine (depending on mode)
- Right-click: Cancel current placement / Exit build mode
- ESC: Cancel current placement / Exit build mode
- R: Rotate ghost 90° clockwise
- 1-9 keys: Quick-select build menu items (future)
- B: Toggle build menu (future)

### Outputs:
- Ghost preview sprite following mouse
- Placed conveyor sprites on grid
- Visual feedback for valid/invalid placement

### State changes:
- TileOccupancy updated when placing/demolishing
- List of placed machines updated
- Current build mode (None, Build, Demolish)
- Selected machine type
- Ghost rotation

### Failure cases:
- Placing on occupied tile → show red ghost, block placement
- Placing outside grid → hide ghost
- No machine selected → no ghost shown

## Data

### TileOccupancy
```csharp
// src/Assets/Simulation/TileOccupancy.cs
public class TileOccupancy
{
    private Dictionary<Vector2Int, string> occupiedTiles; // tile -> machineId

    public bool IsOccupied(Vector2Int pos);
    public bool CanPlace(Vector2Int pos, Vector2Int size);
    public void Occupy(Vector2Int pos, string machineId);
    public void Clear(Vector2Int pos);
}
```

### PlacedMachine
```csharp
public class PlacedMachine
{
    public string Id;           // Unique instance ID
    public string MachineTypeId; // "Conveyor", "Splitter", etc.
    public Vector2Int Position;
    public int Rotation;        // 0, 90, 180, 270
}
```

## UI/UX

### Build Menu (simple version for M0)
- Floating panel or bottom bar
- Shows available machines as buttons
- Conveyor button with icon/text
- Demolish button

### Ghost Preview
- Semi-transparent sprite at mouse position
- Snaps to grid
- Green tint when valid, red tint when invalid
- Rotates with R key
- Arrow indicator showing direction

### Placed Conveyors
- Solid sprite on grid
- Shows direction arrow
- Highlighted when hovered in demolish mode

### Debug overlay additions
- Current mode: None/Build/Demolish
- Selected machine type
- Ghost rotation

## Acceptance criteria
- [ ] Build menu appears with Conveyor and Demolish options
- [ ] Selecting Conveyor shows ghost preview at mouse position
- [ ] Ghost snaps to grid tiles
- [ ] R key rotates ghost 90° clockwise
- [ ] Left-click places conveyor on valid tile
- [ ] Placed conveyor persists and shows on grid
- [ ] Cannot place on already occupied tile (red ghost)
- [ ] Right-click or ESC cancels placement mode
- [ ] Demolish mode: clicking conveyor removes it
- [ ] Debug overlay shows current mode and rotation
- [ ] Multiple conveyors can be placed
- [ ] 60 FPS maintained with 100+ placed conveyors

## Test plan

### Manual:
1. Enter play mode - build menu should appear
2. Click Conveyor button - ghost should appear at mouse
3. Move mouse - ghost follows and snaps to grid
4. Press R multiple times - ghost rotates (0°, 90°, 180°, 270°)
5. Left-click on empty tile - conveyor placed
6. Try to place on same tile - should show red and block
7. Place 5+ conveyors in a line
8. Press ESC - exit build mode, no ghost
9. Click Demolish button - enter demolish mode
10. Click on conveyor - it should be removed
11. Press F3 - debug shows mode and rotation

### Edge cases:
- Place at grid boundary
- Rapid clicking (no duplicates)
- Rotate while placing
- Switch between build/demolish rapidly
