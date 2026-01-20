# Coding Standards (engine-agnostic)

## Principles
- Data-driven definitions in JSON/YAML (decide per engine).
- Deterministic simulation ticks (fixed timestep).
- Separation:
  - Simulation core (headless-friendly)
  - Presentation (sprites, UI)
  - Input/controller layer

## Naming
- Items: PascalCase IDs (`SteelIngot`) + display name in data.
- Machines: PascalCase IDs (`MagnetSeparator`) + category.

## Logging
- Warnings for missing data references.
- Debug overlays for item flow & machine states.

## Testing
- Simulation unit tests for machine logic where feasible.
- Golden test maps/data snapshots for regression.
