# CHANGELOG
# Machine-readable release history.

## 0.1.0
- Initial AI governance scaffold.

## 0.1.1
- Added GIT and DEBUG agent roles with governance updates.

## 0.1.2
- Clarified continuous execution behavior and stopping-point requirements.

## 0.1.3
- Enforced consistent agent declaration and pre-task switch announcements.

## 0.1.4
- Added spec template and minimum spec checks.
- Added work plan format guidance for step reporting.

## 0.1.5
- Renamed /specs to /plans for planning docs.
- Added /spec for implementation specs with a template and CI checks.

## 0.1.6
- Added /templates for root NAME.md templates and plan/spec templates.
- Standardized naming rules and updated CI/README to reflect template usage.

## 0.1.7
- Standardized implementation spec to /spec/SPECIFICATION.md and aligned template/CI checks.

## 0.2.0
- Added HSVXP core library with parsing, palette generation, and output formatting.
- Added CLI with config loading, random/invert, multiplier, and swatch generation.
- Added unit tests for parser, palette rules, and config handling.

## 0.2.1
- Switched CLI argument parsing to System.CommandLine.
- Added Windows PowerShell completion registration and ANSI support detection.
- Added tests for completion script builder and ANSI support logic.

## 0.2.2
- Added AOT-safe JSON serialization configuration.
- Expanded test coverage for color math and output formatting.
- Added project README with example outputs.

## 0.2.3
- Updated multiplier to iterate palettes per column using previous highlight.
- Swatch layout now renders as 5xN grid with 16px default tile size.

## 0.2.4
- Routed CLI text output through the shared palette formatter/writer.

## 0.2.5
- Enforced minimum swatch tile size based on label measurements.

## 1.0.1
- Added swatch orientation option to render palettes as rows or columns.
- Standardized swatch label formatting to use rgb(...) and hsv(...).

## 1.0.0
- Finalized CLI output as a clean table with group dividers.
- Confirmed swatch grid layout and minimum tile size enforcement.
- Updated README and spec with table sample output and swatch example.
- Built distribution artifacts (exe and core DLL).

## 0.2.6
- Set default square size to the minimum baseline (32px).
