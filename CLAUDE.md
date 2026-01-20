# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Junkyard Automation is a 2D isometric factory automation game built with **Unity (C#)**. Players transform messy scrap batches into standardized products through sorting and processing pipelines.

## Tech Stack

- **Engine:** Unity 6.3 with 2D URP
- **Language:** C#
- **UI:** Unity UI Toolkit
- **Data:** JSON files in `Assets/StreamingAssets/data/`
- **Source:** All C# code in `src/` (symlinked/copied to `Assets/Scripts/`)
- **Simulation:** Deterministic fixed timestep (20 ticks/second)

## Architecture

The simulation is **deterministic and separable from rendering**. Key separation:

- **Simulation** (`src/Simulation/`): Pure game state, tick-based updates
- **Presentation** (`src/Core/`, sprites): Renders from simulation snapshots
- **UI** (`src/UI/`): Observes state via events, never modifies directly

**Tick order:** DeliverySystem → ConveyorSystem → MachineSystem → ContractSystem

**Coordinate system:** 2:1 isometric (64x32 tiles). Use `GridSystem` for all conversions between screen, world, and grid coordinates.

## Golden Rules

From `PROJECT/AGENTS.md`:
- Prefer simple, composable systems over cleverness
- Everything data-driven where feasible (machines/items/recipes/contracts in JSON)
- **No hidden randomness** - all variability must be forecasted and visible
- Keep vertical slice small and playable

## Workflow

1. **Spec first:** Create feature spec using `PROJECT/PROMPTS/feature_spec_template.md`
2. **Design docs:** Update `DESIGN/` if gameplay changes
3. **Implement:** Match spec + reference design docs
4. **Test:** Add acceptance criteria per `QA/TEST_PLAN.md`
5. **PR:** Include what changed, how to test, screenshots if UI

## Definition of Done

A feature is done when (from `PROJECT/DEFINITION_OF_DONE.md`):
1. Behavior specified in design docs or feature spec
2. Data-driven config where applicable
3. Has acceptance criteria + manual test steps
4. No new warnings/errors on boot
5. UI has tooltips/labels for comprehension
6. Maintains target FPS (500+ items, 100+ machines)
7. Save/load compatibility supported or versioned

## Key Documentation

| Doc | Purpose |
|-----|---------|
| `GAME_VISION.md` | Pillars, player fantasy, success criteria |
| `PROJECT/AGENTS.md` | AI agent operating manual (golden rules) |
| `PROJECT/MILESTONES.md` | M0-M3 scope definitions |
| `ENGINE/ARCHITECTURE.md` | Class layout, tick behavior, state management |
| `ENGINE/DATA_FORMATS.md` | JSON schemas for items, machines, contracts |
| `DESIGN/GAMEPLAY_LOOP.md` | Core loop, player actions |

## Branching

- `main`: Always bootable without errors
- `feature/*`: One feature at a time
- `fix/*`: Bug fixes
