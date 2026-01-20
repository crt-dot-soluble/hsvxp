# MEMORY-LEDGER
# Append-only immutable memory log.
# Record significant decisions, actions, and results.

- Repo bootstrapped with AI governance scaffold.
- Added GIT and DEBUG agent roles with updated governance files and tasks.
- Clarified continuous execution behavior and stopping-point requirement across governance files.
- Enforced consistent agent declaration and pre-task switch announcements.
- Added spec template, spec presence checks, and work plan format guidance.
- Renamed /specs to /plans, added /spec with template, and updated governance protocol/CI checks for new folder roles.
- Added /templates for root NAME.md and plan/spec templates; standardized naming rules and updated CI/README accordingly.
- Standardized implementation spec to /spec/SPECIFICATION.md and renamed template to /templates/SPECIFICATION.md.
- Implemented initial HSVXP solution: core library, CLI app, and tests.
- Set config defaults: square size 64, multiplier 1, output name prefix "hsvxp".
- Defined output formats: text line layout, JSON array with hsv/rgb/hex, raw lines as "NAME H,S,V".
- Swatch naming auto-appends .jpg when missing; uses default prefix if name not provided.
- Clipboard output is best-effort on Windows only; argument parse errors map to invalid color error message.
- Migrated CLI parsing to System.CommandLine and added Windows PowerShell completion registration.
- Added ANSI support detection (best-effort) and tests for ANSI logic and completion script generation.
- Added AOT-safe JSON serialization by wiring DefaultJsonTypeInfoResolver.
- Expanded test coverage for color math and output formatting.
- Added project README (and template) with real CLI output examples.
- Updated swatch rendering to 5xN grid and restored 16px default tile size.
- Multiplier now iterates palettes by seeding next column from previous highlight.
- Routed CLI text output through shared PaletteConsoleWriter for consistent formatting.
- Added external terminal task using PowerShell stop-parsing to pass #RRGGBB safely.
- Swatch tiles now enforce a minimum size required to fit all text labels.
- Default square size set to 32 so automatic minimum sizing applies by default.
- Updated README/spec to reflect table output and swatch grid defaults.
- Prepared distribution artifacts (exe and core DLL) for v1.0.0 release.
- Documented distribution artifacts in README.
- Added swatch orientation flag to swap rows/columns and standardized swatch labels to rgb(...) and hsv(...).
