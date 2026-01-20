# Architecture

## Core principle
Simulation is deterministic and separable from rendering.

## Project Layout

```
src/                            # Godot project root
├── project.godot
├── JunkyardAutomation.csproj   # C# project file
├── JunkyardAutomation.sln      # Visual Studio solution
├── scripts/
│   ├── autoload/               # Singletons (autoloaded)
│   │   ├── GameManager.cs      # Main game state, initialization
│   │   ├── GridSystem.cs       # Isometric grid + coordinate conversion
│   │   ├── SimulationManager.cs # Fixed timestep tick loop
│   │   └── ContentRegistry.cs  # Loaded definitions access
│   ├── core/
│   │   ├── CameraController.cs # Pan, zoom, bounds
│   │   ├── GridRenderer.cs     # Procedural grid drawing
│   │   └── TileHighlighter.cs  # Mouse hover tile highlight
│   ├── data/
│   │   ├── ItemDefinition.cs   # Item data structure
│   │   ├── MachineDefinition.cs # Machine data structure
│   │   └── RecipeDefinition.cs # Recipe data structure
│   ├── simulation/
│   │   ├── YardState.cs        # Current yard state
│   │   ├── PlacedMachine.cs    # Runtime machine instance
│   │   ├── ItemEntity.cs       # Runtime item instance
│   │   ├── ConveyorSystem.cs   # Moves items along conveyors
│   │   ├── MachineSystem.cs    # Processes items in machines
│   │   └── DeliverySystem.cs   # Spawns scrap batches
│   ├── placement/
│   │   ├── PlacementManager.cs # Build/demolish mode
│   │   ├── PlacementGhost.cs   # Preview sprite + validation
│   │   └── MachineVisual.cs    # Visual representation of machines
│   └── ui/
│       ├── BuildMenu.cs        # Machine selection
│       ├── DebugOverlay.cs     # Debug info (coords, FPS)
│       └── Hud.cs              # Main HUD controller
├── scenes/
│   ├── main.tscn               # Main game scene
│   ├── game/
│   │   ├── grid.tscn           # Grid rendering
│   │   ├── machine.tscn        # Machine prefab
│   │   └── item.tscn           # Item prefab
│   └── ui/
│       ├── build_menu.tscn
│       └── debug_overlay.tscn
├── data/
│   ├── items.json
│   ├── machines.json
│   └── recipes.json
└── assets/
    └── sprites/
```

## Key Autoloads (Singletons)

### GridSystem
Handles isometric coordinate conversions:
- `WorldToGrid(Vector2 worldPos)` - World position to grid cell
- `GridToWorld(Vector2I gridPos)` - Grid cell to world center
- `ScreenToGrid(Vector2 screenPos)` - Screen pixel to grid cell
- Configurable tile size (e.g., 64x32 pixels for 2:1 isometric)

```csharp
// GridSystem.cs
public partial class GridSystem : Node
{
    public int TileWidth { get; set; } = 64;
    public int TileHeight { get; set; } = 32;
    public int GridWidth { get; set; } = 32;
    public int GridHeight { get; set; } = 32;

    public Vector2 GridToWorld(Vector2I gridPos)
    {
        float x = (gridPos.X - gridPos.Y) * TileWidth / 2.0f;
        float y = (gridPos.X + gridPos.Y) * TileHeight / 2.0f;
        return new Vector2(x, y);
    }

    public Vector2I WorldToGrid(Vector2 worldPos)
    {
        float x = (worldPos.X / (TileWidth / 2.0f) + worldPos.Y / (TileHeight / 2.0f)) / 2.0f;
        float y = (worldPos.Y / (TileHeight / 2.0f) - worldPos.X / (TileWidth / 2.0f)) / 2.0f;
        return new Vector2I(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }
}
```

### SimulationManager
- Runs in `_PhysicsProcess` at configurable tick rate
- Calls systems in order: Delivery → Conveyor → Machine → Contract
- Emits `TickCompleted` signal for UI updates
- Supports pause/resume

```csharp
// SimulationManager.cs
public partial class SimulationManager : Node
{
    [Signal]
    public delegate void TickCompletedEventHandler(int tickNumber);

    public int CurrentTick { get; private set; } = 0;
    public bool IsPaused { get; private set; } = false;

    public override void _PhysicsProcess(double delta)
    {
        if (IsPaused) return;
        ExecuteTick();
    }

    private void ExecuteTick()
    {
        CurrentTick++;
        // Run systems in order
        // DeliverySystem?.Tick();
        // ConveyorSystem?.Tick();
        // MachineSystem?.Tick();
        EmitSignal(SignalName.TickCompleted, CurrentTick);
    }
}
```

## Data Classes

### PlacedMachine
```csharp
public class PlacedMachine
{
    public string Id { get; set; }              // Unique instance ID
    public string TypeId { get; set; }          // Reference to MachineDefinition
    public Vector2I GridPosition { get; set; }
    public int Rotation { get; set; }           // 0, 90, 180, 270
    public MachineState State { get; set; }     // Running, Blocked, Stalled
    public List<ItemEntity> InputBuffer { get; } = new();
    public List<ItemEntity> OutputBuffer { get; } = new();
    public float ProcessingTimer { get; set; }
    public Dictionary<string, object> Config { get; } = new();
}
```

### ItemEntity
```csharp
public class ItemEntity
{
    public string Id { get; set; }              // Unique instance ID
    public string TypeId { get; set; }          // Reference to ItemDefinition
    public Vector2I GridPosition { get; set; }
    public float TileProgress { get; set; }     // 0-1 progress across current tile
    public int Direction { get; set; }          // Movement direction (0, 90, 180, 270)
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
- UI observes state changes via signals, never modifies directly

## Scene Structure

Main scene hierarchy:
```
Main (Node2D)
├── Camera2D (with CameraController.cs)
├── Grid (Node2D, with GridRenderer.cs)
│   └── TileHighlighter (Node2D)
├── Machines (Node2D, container for placed machines)
├── Items (Node2D, container for items)
└── UI (CanvasLayer)
    ├── BuildMenu
    └── DebugOverlay
```
