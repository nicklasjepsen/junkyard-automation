# Tileset Spec

## Grid
- Tile size: pick one and standardize early:
  - Option: 64x32 for ground diamond (common isometric)
  - Height layers via sprite y-offset.

## Layers
- Ground
- Decals (paint, stains, arrows)
- Buildings/machines
- Items (on top of belts)
- UI overlays (not part of tileset)

## Collision/occupancy
- Each machine occupies 1+ tiles.
- Conveyors occupy 1 tile.
- Splitter occupies 1 tile.
