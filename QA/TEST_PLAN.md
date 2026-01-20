# Test Plan

## Smoke tests (every build)
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
