# Junkyard Automation (2D Isometric)

A 2D isometric factory automation game where inputs are messy scrap batches. The player builds adaptive sorting + processing lines to convert unpredictable junk into standardized products and fulfill contracts.

## Tech Stack
- **Engine:** Unity 6.3 (C#)
- **Rendering:** 2D URP with Tilemap
- **UI:** Unity UI Toolkit

## High-level loop
Receive scrap batch → sort (filters/sensors) → process (wash/shred/smelt/grind) → assemble products → sell/fulfill contracts → unlock better tech.

## Target experience
- The joy of turning chaos into clean throughput.
- Bottlenecks are readable and fixable.
- Randomness is forecasted and supplier-driven (chaos is a choice).

## Build goals (vertical slice)
A 10–15 minute playable loop:
- 4 scrap categories: Ferrous, NonFerrous, Plastic, Trash
- Basic machines: Conveyor, Splitter, Magnet, Washer, Shredder, Smelter, Storage, Seller/Loading Dock
- Products: Steel Ingot, Copper Chunk, Plastic Pellets
- Contracts: 3 simple contracts, penalties for overflow/backlog

## Docs
Start here:
- GAME_VISION.md
- DESIGN/GAMEPLAY_LOOP.md
- PROJECT/AGENTS.md
- ENGINE/TECH_STACK.md

## Non-goals (for v1)
- Multiplayer
- Full story campaign
- Terrain deformation
- Complex NPC AI

## License
TBD by repo owner.
