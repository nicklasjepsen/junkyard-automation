# Gameplay Loop

## Day cycle (v1)
- Player receives scheduled deliveries (randomized by supplier profile).
- Player runs the yard continuously; time can be paused.
- Player fulfills contracts by producing and shipping required products.

## Player actions
- Place machines and belts
- Configure routing rules on splitters (v1: basic)
- Watch bottlenecks via overlays
- Sell excess output
- Accept contracts

## Win/lose
- No hard "game over" in v1.
- Failure pressure:
  - Yard overflow triggers fees
  - Contract deadlines can fail and reduce reputation (v1: money penalty only)

## Fun complications (v1)
- Contamination causes washers to bottleneck.
- Non-matching items clog lines unless filtered.
- Storage fills, blocking upstream.
