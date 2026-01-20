# Tech Stack

## Engine: Godot 4.x (C#/.NET)

**Version:** Godot 4.3+ with .NET 8.0

## Requirements
- 2D isometric rendering with many moving items.
- Data-driven content loading.
- Fixed timestep simulation.
- Save/load.
- UI: build menus, inspectors, overlays.

## Godot-Specific Choices

### Rendering
- **2D Renderer** with isometric tilemap support
- **TileMap** with isometric mode for grid-based terrain/floors
- **Sprite2D** for machines and items
- **CanvasLayers** for UI separation
- **Y-Sort** enabled for proper depth ordering

### Simulation
- **_physics_process()** for deterministic tick-based simulation
- Custom SimulationManager autoload for tick coordination
- Target: 20 simulation ticks/second (configurable via Engine.physics_ticks_per_second)

### Data Loading
- **JSON** via Godot's JSON class
- Data files in `data/` folder (exported with project)
- Resource files (.tres) for editor-friendly definitions (optional)

### UI
- **Control** nodes for HUD/menus
- **CanvasLayer** for HUD overlay
- **Theme** resources for consistent styling

### Project Structure
```
src/                            # Godot project root
├── project.godot               # Project configuration
├── autoload/                   # Singletons (GameManager, etc.)
├── scenes/
│   ├── main.tscn              # Main game scene
│   ├── game/                   # Game-specific scenes
│   └── ui/                     # UI scenes
├── scripts/
│   ├── core/                   # Grid, camera, rendering
│   ├── data/                   # Data loaders, definitions
│   ├── simulation/             # Runtime entities and systems
│   ├── placement/              # Building/placement logic
│   └── ui/                     # UI scripts
├── data/                       # JSON definitions
│   ├── items.json
│   ├── machines.json
│   └── recipes.json
├── assets/
│   ├── sprites/
│   ├── fonts/
│   └── audio/
└── resources/                  # Godot resource files (.tres)
```

### Autoloads (Singletons)
- `GameManager` - Game state, initialization
- `GridSystem` - Coordinate conversions, grid data
- `SimulationManager` - Tick loop, simulation systems
- `ContentRegistry` - Loaded definitions access

### Dependencies
- .NET 8.0 SDK
- Godot.NET.Sdk 4.3.0
- Keep external packages minimal
- Core Godot + .NET functionality only

## Performance Notes
- Use object pooling for items (custom implementation)
- Leverage Godot's built-in batching for sprites
- Profile early with 500+ items
- Consider using typed arrays and packed scenes
