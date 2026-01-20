# Feature Spec: Simulation & Item Movement

## Goal
Implement the core simulation loop and item movement system, enabling items to move along conveyor belts in a deterministic, tick-based fashion.

## Non-goals
- Processing machines (shredder, washer, etc.) - future PR
- Delivery dock scheduling - simplified spawn for testing
- Splitter routing logic - future PR
- Item stacking/merging - future PR

## Design references
- ENGINE/ARCHITECTURE.md - SimulationManager, ItemEntity, ConveyorSystem
- DESIGN/SYSTEMS_SORTING.md - Conveyor behavior
- DESIGN/CONTENT_CATALOG.md - Conveyor ticksPerMove

## User stories
- As a player, I can see items spawned on the grid
- As a player, I can see items move along conveyors
- As a player, I can observe items transfer between connected conveyors
- As a player, I can see items stop at conveyor ends (no output)

## Behavior

### Simulation Tick
- Fixed timestep simulation running in FixedUpdate
- Default tick rate: 20 ticks per second (50ms per tick)
- Simulation can be paused/resumed
- All systems update in deterministic order

### Item Movement
- Items have position (grid cell) and progress (0-1 within cell)
- Each tick, items advance by (1.0 / ticksPerMove)
- When progress >= 1.0, item moves to next cell
- If next cell is blocked, item waits (progress stays at 1.0)

### Conveyor Handoff
- Conveyor outputs to tile in its facing direction
- If output tile has a conveyor facing away, item transfers
- If output tile is empty or blocked, item waits at edge
- Items despawn when reaching a tile with no valid output (for now)

### Visual Representation
- Items rendered as colored diamonds on their tile
- Item position interpolated between ticks for smooth movement
- Item color based on ItemDefinition from registry

## Data Structures

### ItemEntity
```csharp
public class ItemEntity
{
    public string Id;              // Unique instance ID
    public string TypeId;          // Reference to ItemDefinition
    public Vector2Int Position;    // Current grid cell
    public float Progress;         // 0-1 progress across tile
    public int Direction;          // Movement direction (from conveyor)
}
```

### SimulationManager
```csharp
public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance;
    public int TicksPerSecond = 20;
    public bool IsPaused { get; set; }
    public long CurrentTick { get; private set; }

    public event Action<long> OnTick;
}
```

### ConveyorSystem
```csharp
public static class ConveyorSystem
{
    public static void Tick(YardState yard, List<ItemEntity> items);
}
```

## Implementation

### SimulationManager.cs
- Singleton MonoBehaviour
- Uses FixedUpdate with Time.fixedDeltaTime = 1/TicksPerSecond
- Calls systems in order: ConveyorSystem
- Fires OnTick event after each tick

### ItemEntity.cs
- Simple data class for item runtime state
- Methods: GetWorldPosition() for rendering interpolation

### ConveyorSystem.cs
- Static class with Tick() method
- Iterates all items, advances progress
- Handles handoffs between conveyors
- Removes items that reach dead ends (for now)

### ItemVisualManager.cs
- Creates/updates/destroys item visuals
- Interpolates position between ticks for smooth movement
- Uses item color from ContentRegistry

### YardState additions
- List<ItemEntity> Items
- Methods: AddItem(), RemoveItem(), GetItemsAt()

## Test Spawning
For testing, add a simple spawn mechanism:
- Press SPACE to spawn a ScrapFerrous item at mouse position
- Item inherits direction from conveyor it's placed on

## Acceptance Criteria
- [ ] SimulationManager runs at 20 ticks/second
- [ ] Items can be spawned on conveyors (SPACE key)
- [ ] Items move along conveyors smoothly
- [ ] Items transfer between connected conveyors
- [ ] Items stop at conveyor ends
- [ ] Item colors match their type from registry
- [ ] Debug overlay shows current tick count
- [ ] P key pauses/resumes simulation

## Test Plan

### Manual
1. Run Setup Game Scene
2. Place 5+ conveyors in a line (same direction)
3. Press SPACE on first conveyor - item appears
4. Item moves along all conveyors
5. Item stops at last conveyor (or despawns)
6. Press P - simulation pauses
7. Press P - simulation resumes
8. F3 shows tick count incrementing

### Edge cases
- Place item on non-conveyor tile (should not spawn or immediately despawn)
- Conveyor facing into another conveyor's side (no handoff)
- Two conveyors facing each other (items block)
- Very long conveyor chain (100+ tiles)
