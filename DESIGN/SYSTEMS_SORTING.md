# Sorting Systems

## Goals
- Make sorting the primary puzzle.
- Allow robust designs that handle variability.

## Items (v1 categories)
- ScrapFerrous
- ScrapNonFerrous
- ScrapPlastic
- ScrapTrash

(Internally items may be instances with properties; v1 can be category-only.)

## Machines (v1)
- Conveyor: moves items forward one tile per N ticks.
- Splitter: 1 input, 2 outputs; routes items based on filter.
- MagnetSeparator: pulls ScrapFerrous to a side output, others pass through.
- Storage: buffers items (acts as sink/source).

## Routing rules (v1)
- Splitter has a filter list:
  - if item in Filter → Output A
  - else → Output B

## Overlays
- Flow arrows
- Blocked output indicators
- Storage fullness
