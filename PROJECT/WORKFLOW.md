# Workflow

## Branching
- main: always playable or at least bootable without errors.
- feature/*: one feature at a time.
- fix/*: bug fixes.

## PR discipline
- Each PR must include:
  - What changed
  - How to test
  - Screenshots/gifs if UI
  - Any new data formats documented in ENGINE/DATA_FORMATS.md
- PR review checklist: `PROJECT/PROMPTS/pr_review_checklist.md`

## Spec first
If a feature changes gameplay behavior, update or add docs in DESIGN/ first.
