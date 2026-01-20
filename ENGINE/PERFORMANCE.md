# Performance

## Targets
- Stable FPS on a modest yard with:
  - 500–2000 items in motion
  - 100+ machines

## Strategies
- Avoid per-item expensive operations every frame.
- Update rendering from simulation snapshots.
- Batch draw items by sprite/atlas when possible.
- Provide debug counter overlays:
  - items active
  - machine ticks
  - blocked count
