# Test Plan

## M0 - Grid Foundation Tests

### Boot & Render
1. Unity project opens without errors or warnings
2. Play mode starts and shows isometric grid
3. Grid renders as checkerboard diamond pattern
4. 60 FPS maintained with 32x32 grid

### Camera Controls
1. WASD keys pan camera smoothly
2. Arrow keys pan camera smoothly
3. Middle mouse drag pans camera
4. Mouse wheel zooms in/out
5. Zoom respects min/max limits (stops at boundaries)
6. Camera stays within grid bounds

### Tile Highlighting
1. Moving mouse over grid highlights single tile
2. Highlight follows mouse accurately
3. Moving mouse outside grid hides highlight
4. Coordinate shown in debug matches visual tile

### Debug Overlay
1. F3 toggles debug overlay visibility
2. Shows correct grid coordinates for hovered tile
3. Shows world position
4. Shows FPS counter
5. Shows current zoom level

## M0 - Conveyor Placement Tests

### Build Menu
1. Build menu appears at bottom of screen
2. Conveyor button is clickable
3. Demolish button is clickable
4. Cancel button appears when in build/demolish mode

### Placement Ghost
1. Clicking Conveyor shows green ghost at mouse position
2. Ghost snaps to grid tiles
3. Ghost follows mouse movement
4. R key rotates ghost (0° → 90° → 180° → 270° → 0°)
5. Ghost arrow points in rotation direction

### Placing Conveyors
1. Left-click on empty tile places conveyor
2. Placed conveyor shows on grid with direction arrow
3. Placed conveyor persists (doesn't disappear)
4. Cannot place on occupied tile (ghost turns red)
5. Multiple conveyors can be placed in sequence
6. ESC cancels build mode

### Demolish Mode
1. Click Demolish button enters demolish mode
2. Hovering over conveyor shows orange highlight
3. Left-click on conveyor removes it
4. Tile becomes available after demolish
5. Can place new conveyor on demolished tile

### Edge Cases
1. Rapid clicking doesn't create duplicates
2. Placing at grid boundaries works
3. 100+ conveyors maintains 60 FPS

## M1 - Smoke tests (every build)
1. Game boots to yard
2. Can place conveyor
3. Delivery spawns scrap
4. Scrap moves on conveyors
5. Can build splitter and set filter
6. Processing produces products
7. Seller sells products or contract counts them
8. Save and load preserves layout and inventories

## Regression tests (weekly)
- Overflow/backlog penalty triggers reliably
- Machine blocked state appears and clears when resolved
- No item duplication/loss across conveyor junctions

## Balance checks
- First contract completable in 8–12 minutes by new player
