# Save/Load

## Requirements (v1)
- Save yard layout (machines, rotation, positions)
- Save inventories (items in buffers, storage)
- Save money + active contracts + delivered quantities

## Approach
- Serialize simulation state only (not rendering).
- Keep stable ids for machines/items.

## Compatibility
- Use dataVersion in save.
- If ids missing due to changes, attempt graceful migration or mark save incompatible.
