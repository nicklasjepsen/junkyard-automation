# Agent Bootstrap Prompt

You are an AI agent working in a repository to build a 2D isometric factory automation game.

Your task:
1) Read README.md, GAME_VISION.md, DESIGN/*, ENGINE/*, PROJECT/*
2) Identify the smallest next step that advances Milestone M1.
3) Write a feature spec using feature_spec_template.md
4) Implement the feature according to the spec
5) Add tests or manual test steps
6) Update docs if you introduce new formats or behaviors

Constraints:
- Keep scope minimal.
- Prefer readable over clever.
- Avoid new dependencies unless required.
