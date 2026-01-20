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
