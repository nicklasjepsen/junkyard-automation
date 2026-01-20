# Architecture

## Core principle
Simulation is deterministic and separable from rendering.

## Project Layout

```
src/Assets/Scripts/             # All C# source code
├── Core/
│   ├── GameManager.cs          # Main game loop, state management
│   ├── SimulationManager.cs    # Fixed timestep tick loop
│   ├── GridSystem.cs           # Isometric grid + coordinate conversion
│   ├── GridRenderer.cs         # Procedural grid mesh generation
│   ├── TileHighlighter.cs      # Mouse hover tile highlight
│   └── CameraController.cs     # Pan, zoom, bounds
├── Data/
│   ├── DataLoader.cs           # JSON loading from StreamingAssets
│   ├── ItemDefinition.cs       # Item data structure
│   ├── MachineDefinition.cs    # Machine data structure
│   └── ContractDefinition.cs   # Contract data structure
├── Simulation/
│   ├── YardState.cs            # Current yard state (tiles, machines, items)
│   ├── TileOccupancy.cs        # What occupies each grid cell
│   ├── MachineEntity.cs        # Runtime machine instance
│   ├── ItemEntity.cs           # Runtime item instance
│   └── Systems/
│       ├── ConveyorSystem.cs   # Moves items along conveyors
│       ├── MachineSystem.cs    # Processes items in machines
│       ├── ContractSystem.cs   # Tracks contract progress
│       └── DeliverySystem.cs   # Spawns scrap batches
├── Placement/
│   ├── PlacementManager.cs     # Build/demolish mode
│   ├── PlacementGhost.cs       # Preview sprite + validation
│   └── PlacementValidator.cs   # Checks valid placement
├── UI/
│   ├── HUDController.cs        # Main HUD root
│   ├── BuildMenuUI.cs          # Machine selection
│   ├── InspectorPanelUI.cs     # Selected machine details
│   ├── ContractPanelUI.cs      # Active contracts
│   ├── DebugOverlayUI.cs       # Debug info (coords, FPS, counts)
│   └── TooltipUI.cs            # Hover tooltips
└── Utils/
    ├── IsometricUtils.cs       # Coordinate conversion helpers
    ├── ObjectPool.cs           # Generic object pooling
    └── Extensions.cs           # C# extension methods
```

## Key Classes

### GridSystem
Handles isometric coordinate conversions:
- `Vector2Int WorldToGrid(Vector3 worldPos)` - World position to grid cell
- `Vector3 GridToWorld(Vector2Int gridPos)` - Grid cell to world center
- `Vector2Int ScreenToGrid(Vector2 screenPos)` - Screen pixel to grid cell
- Configurable tile size (e.g., 64x32 pixels for 2:1 isometric)

### SimulationManager
- Runs in `FixedUpdate` at configurable tick rate
- Calls systems in order: Delivery → Conveyor → Machine → Contract
- Exposes `SimulationTick` event for UI updates
- Supports pause/resume

### MachineEntity
```csharp
public class MachineEntity
{
    public string Id;                    // Unique instance ID
    public string TypeId;                // Reference to MachineDefinition
    public Vector2Int GridPosition;
    public int Rotation;                 // 0, 90, 180, 270
    public MachineState State;           // Running, Blocked, Stalled
    public Queue<ItemEntity> InputBuffer;
    public Queue<ItemEntity> OutputBuffer;
    public float ProcessingTimer;
    public Dictionary<string, object> Config; // e.g., splitter filters
}
```

### ItemEntity
```csharp
public class ItemEntity
{
    public string Id;                    // Unique instance ID
    public string TypeId;                // Reference to ItemDefinition
    public Vector2Int GridPosition;
    public float TileProgress;           // 0-1 progress across current tile
    public int Direction;                // Movement direction
}
```

## Tick Behavior

Each simulation tick:
1. **DeliverySystem**: Spawn items at delivery dock if scheduled
2. **ConveyorSystem**: Move items along belts, handle handoffs
3. **MachineSystem**: Pull inputs, process, push outputs
4. **ContractSystem**: Check completions, update deadlines

## State Management

- `YardState` holds all runtime state (serializable for save/load)
- Definitions (items, machines) are read-only after load
- UI observes state changes via events, never modifies directly
