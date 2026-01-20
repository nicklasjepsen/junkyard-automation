# Tech Stack

This document is intentionally engine-agnostic.

## Requirements
- 2D isometric rendering with many moving items.
- Data-driven content loading.
- Fixed timestep simulation.
- Save/load.
- UI: build menus, inspectors, overlays.

## Suggested engines (choose one)
- Godot (fast iteration, good 2D)
- Unity (common tooling, good UI)
- MonoGame (code-first, excellent control)

## Recommendation
Pick an engine that the team can ship with quickly. For AI-agent development, prioritize:
- Simple scene setup
- Easy JSON/YAML loading
- Straightforward UI

Once chosen, update:
- ENGINE/ARCHITECTURE.md with concrete classes/modules.
- ENGINE/DATA_FORMATS.md with exact schemas.
