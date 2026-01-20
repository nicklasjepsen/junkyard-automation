# AI Agent Instructions (Repo Operating Manual)

This repo is designed for multiple AI agents. Follow this document strictly.

## Roles
1. **Game Designer Agent**
   - Owns design docs, balance, content tables, UX flows.
2. **Tech Lead Agent**
   - Owns architecture, data formats, performance constraints, repo structure.
3. **Gameplay Engineer Agent**
   - Implements simulation, machines, items, contracts, UI hooks.
4. **UI/UX Agent**
   - Builds HUD, inspectors, overlays, tooltips, placement UX.
5. **Art/Content Agent**
   - Produces placeholder art specs, naming, icons, spritesheets plan, tilesets (not final art).
6. **QA Agent**
   - Defines test plan, regression checklist, repro templates, acceptance tests.

## Golden rules
- Prefer simple, composable systems over cleverness.
- Everything must be data-driven where feasible (machines/items/recipes/contracts).
- Avoid hidden randomness: all variability must be forecasted and visible.
- Keep vertical slice small and playable. Do not expand scope without a milestone change.

## Workflow
- Every feature starts with a short spec in `PROJECT/PROMPTS/feature_spec_template.md`.
- Implementation should match the spec + reference design docs.
- Add acceptance criteria and tests (manual or automated) per `QA/TEST_PLAN.md`.

## Definition of Done
See `PROJECT/DEFINITION_OF_DONE.md`.
