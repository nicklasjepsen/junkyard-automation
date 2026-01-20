# Architecture

## Core principle
Simulation is deterministic and separable from rendering.

## Modules
1. Simulation.Core
- Tick loop
- Entities: Item, Machine, Tile
- Systems: ConveyorSystem, MachineSystem, ContractSystem, DeliverySystem

2. Data
- Loads definitions:
  - Items
  - Machines
  - Recipes
  - Contracts

3. Presentation
- Tilemap renderer
- Sprite renderers for machines/items
- Animation state from simulation state

4. UI
- Build menu
- Inspector
- Contracts
- Overlays

## Machine model (suggested)
MachineEntity:
- id, typeId, tile footprint, rotation
- input buffers (queues)
- output buffers (queues)
- state enum: Running/Blocked/Stalled/NoPower (optional)
- processing timer(s)
- config blob (e.g., splitter filter list)

Tick behavior:
- Pull from input if available
- Process for duration
- Push to output if space; else Blocked
