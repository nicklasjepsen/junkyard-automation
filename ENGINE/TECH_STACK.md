# Tech Stack

## Engine: Unity (C#)

**Version:** Unity 6.3

## Requirements
- 2D isometric rendering with many moving items.
- Data-driven content loading.
- Fixed timestep simulation.
- Save/load.
- UI: build menus, inspectors, overlays.

## Unity-Specific Choices

### Rendering
- **2D Renderer** with Universal Render Pipeline (URP) for flexibility
- **Tilemap** system for grid-based terrain/floors
- **SpriteRenderer** for machines and items
- **Sorting Layers:** Background, Floor, Machines, Items, UI

### Simulation
- **FixedUpdate** for deterministic tick-based simulation
- Simulation logic separated from MonoBehaviour where possible
- Target: 20 simulation ticks/second (configurable)

### Data Loading
- **JSON** via Unity's JsonUtility or Newtonsoft.Json
- Data files in `Assets/StreamingAssets/data/` for easy modding later
- ScriptableObjects for editor-friendly definitions (optional)

### UI
- **Unity UI Toolkit** (preferred) or **uGUI** for HUD/menus
- Canvas set to Screen Space - Overlay for HUD
- World Space canvas for in-game labels if needed

### Project Structure
```
src/                        # All C# source code (symlinked into Assets/Scripts)
├── Core/                   # Simulation, grid, coordinate systems
├── Data/                   # Data loaders, definitions
├── Simulation/             # Runtime entities and systems
├── UI/                     # All UI scripts
└── Utils/                  # Helpers, extensions

Assets/
├── Scripts/                # Symlink or copy of src/
├── Prefabs/
├── Scenes/
├── StreamingAssets/
│   └── data/               # JSON definitions
├── Sprites/
└── UI/
```

### Dependencies
- Keep external packages minimal
- Allowed: TextMeshPro (included), Newtonsoft.Json (if needed)
- Avoid: heavy frameworks, paid assets

## Performance Notes
- Use object pooling for items
- Batch sprite rendering where possible
- Profile early with 500+ items
