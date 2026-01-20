# Data Formats

All gameplay definitions must be data-driven where possible.

## Files (suggested)
- data/items.json
- data/machines.json
- data/recipes.json
- data/contracts.json
- data/deliveries.json

## Item schema (example)
- id (string)
- displayName (string)
- category (string)
- sellPrice (int)

## Machine schema (example)
- id
- displayName
- footprint { w, h }
- cost
- inputs { count, bufferSize }
- outputs { count, bufferSize }
- processTime (float)
- machineKind (Conveyor/Splitter/Processor/Storage)
- configSchema (optional)

## Contract schema (example)
- id
- displayName
- productId
- quantity
- rewardMoney
- penaltyMoney
- deadlineDays (optional)

## Versioning
- Include `dataVersion` in save files.
- Guard against missing ids by logging + safe defaults.
