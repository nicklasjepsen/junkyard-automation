# Contracts & Economy

## Money sinks (v1)
- Buying machines
- Tile expansion (optional)
- Overflow fees (yard capacity exceeded)

## Money sources
- Contract completion rewards
- Selling products at market price

## Contracts (v1)
Each contract:
- id
- requested_product_id
- requested_quantity
- deadline_days (optional: v1 can omit and treat as "deliver anytime")
- reward_money
- penalty_money (on cancel/fail)

## Reputation (v1.5)
Optional later: reputation gates better suppliers and contracts.

## Forecast system (post-slice)
Daily forecast UI:
- "More plastics today"
- "Construction debris spike"
Suppliers have distributions for scrap categories.
