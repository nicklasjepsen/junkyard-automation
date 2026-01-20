# Processing Systems

## Goals
- Processing turns categories into products.
- Bottlenecks come from capacity and routing, not hidden math.

## Machines (v1)
- Washer: consumes ScrapPlastic (and optionally others later) and outputs CleanPlastic
- Shredder: consumes ScrapFerrous / ScrapNonFerrous and outputs MetalBits (typed)
- Smelter: consumes MetalBitsFerrous -> SteelIngot; MetalBitsNonFerrous -> CopperChunk
- Pelletizer (optional v1.1): CleanPlastic -> PlasticPellets
  - For v1, Washer can output PlasticPellets directly to reduce scope.

## Inputs/Outputs (v1 simplification)
Option A (simplest):
- Washer: ScrapPlastic -> PlasticPellets
- Shredder: ScrapFerrous -> SteelIngot (via internal steps)
- Shredder: ScrapNonFerrous -> CopperChunk

Option B (more modular, still manageable):
- Washer: ScrapPlastic -> CleanPlastic
- Pelletizer: CleanPlastic -> PlasticPellets
- Shredder: ScrapFerrous -> MetalBitsFerrous
- Smelter: MetalBitsFerrous -> SteelIngot
- Shredder: ScrapNonFerrous -> MetalBitsNonFerrous
- Smelter: MetalBitsNonFerrous -> CopperChunk

Choose B if tech architecture is already solid; otherwise A for fastest slice.

## Jam conditions (v1)
- If output is blocked for > X ticks → machine state "Stalled"
- If input buffer full → upstream blocks
