# Content Catalog

## Items (v1)

### Scrap Categories
| ID | Display Name | Category | Sell Price |
|----|--------------|----------|------------|
| ScrapFerrous | Ferrous Scrap | scrap | $1 |
| ScrapNonFerrous | Non-Ferrous Scrap | scrap | $2 |
| ScrapPlastic | Plastic Scrap | scrap | $1 |
| ScrapTrash | Trash | scrap | $0 |

### Products
| ID | Display Name | Category | Sell Price |
|----|--------------|----------|------------|
| SteelIngot | Steel Ingot | product | $10 |
| CopperChunk | Copper Chunk | product | $15 |
| PlasticPellets | Plastic Pellets | product | $8 |

## Machines (v1)

| ID | Display Name | Size | Cost | Kind |
|----|--------------|------|------|------|
| Conveyor | Conveyor Belt | 1x1 | $10 | Conveyor |
| Splitter | Splitter | 1x1 | $50 | Splitter |
| MagnetSeparator | Magnet Separator | 2x1 | $100 | Processor |
| Washer | Washer | 2x2 | $200 | Processor |
| Shredder | Shredder | 2x2 | $250 | Processor |
| Smelter | Smelter | 2x2 | $300 | Processor |
| Storage | Storage Bin | 2x2 | $75 | Storage |
| Seller | Shipping Dock | 3x2 | $0 | Seller |

## Processing Chains (v1 - Option A simplified)

```
ScrapFerrous → Shredder → Smelter → SteelIngot
ScrapNonFerrous → Shredder → Smelter → CopperChunk
ScrapPlastic → Washer → PlasticPellets
ScrapTrash → (dispose or recycle fee)
```

## Contracts (v1 - examples)

| ID | Product | Quantity | Reward | Penalty |
|----|---------|----------|--------|---------|
| contract_steel_basic | SteelIngot | 10 | $150 | $50 |
| contract_copper_basic | CopperChunk | 5 | $100 | $30 |
| contract_plastic_basic | PlasticPellets | 15 | $150 | $40 |
