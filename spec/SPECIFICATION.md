# SPEC_VERSION: 1.0.3
# APP_NAME: HSVXP
# PURPOSE: Deterministic HSV‑based palette expansion CLI for pixel‑art workflows, supporting strict color input formats, palette multiplication, and standardized labeled swatch tiles.

---

# 1. RUNTIME_TARGET
language: C#
platform: .NET (latest LTS)
build:
  publish_aot: true
  publish_trimmed: true
  trim_mode: full
  publish_single_file: true
  include_native_libs_self_extract: true
runtime_identifiers:
  - win-x64
  - linux-x64
  - osx-arm64

---

# 2. INPUT

## 2.1 Accepted Color Formats (STRICT)
HSVXP only accepts the following formats:

### HEX formats
- #RRGGBB
- #RGB
- hex(#RRGGBB)
- hex(#RGB)

### RGB formats
- rgb(R,G,B)
- rgb(R, G, B)

### HSV formats
- hsv(H,S,V)
- hsv(H, S, V)

## 2.2 Rejected Formats
- Bare R,G,B
- Bare H,S,V
- Any unlisted format

## 2.3 Input Priority
0. If --grayscale-image is present and no color/random is supplied → perform image grayscale only.
1. If --random is present → ignore positional color
2. Else if positional color present → parse
3. Else → error

## 2.4 Parsing Rules
- Case‑insensitive
- Whitespace tolerant
- #FFF expands to #FFFFFF
- RGB values must be 0–255
- HSV values must be H 0–360, S/V 0–100

---

# 3. COLOR_NORMALIZATION
model: HSV
range:
  H: 0–360
  S: 0–100
  V: 0–100

rules:
- MAIN.H = parsed.H
- MAIN.S = parsed.S
- MAIN.V = parsed.V

---

# 4. STANDARD PIXEL‑ART PALETTE STRUCTURE
Roles:
- MAIN
- HIGHLIGHT
- SHADOW
- LIGHT_ACCENT
- DARK_ACCENT

---

# 5. PALETTE_RULES
MAIN:
  H: MAIN.H
  S: MAIN.S
  V: MAIN.V

HIGHLIGHT:
  H: MAIN.H + 5
  S: MAIN.S + 7
  V: MAIN.V + 20

SHADOW:
  H: MAIN.H - 5
  S: MAIN.S - 15
  V: MAIN.V - 27

LIGHT_ACCENT:
  H: MAIN.H + 10
  S: MAIN.S - 15
  V: MAIN.V + 35

DARK_ACCENT:
  H: MAIN.H - 15
  S: MAIN.S - 35
  V: MAIN.V - 45

---

# 5.1 GRAYSCALE_MODE
trigger: --grayscale
rules:
- After input normalization (and optional invert), force MAIN.H = 0 and MAIN.S = 0.
- For all derived roles, set H = 0 and S = 0 (ignore H/S offsets).
- Apply only the V offsets from PALETTE_RULES.
- Output order per multiplier column is darkest-to-lightest based on V (ascending). If V ties, keep the standard role order.
- Multiplier expansion still uses the previous column HIGHLIGHT as the next column MAIN.

---

# 6. CLAMPING
hue wraps at 0/360.
saturation and value clamp to 0–100.

---

# 7. MULTIPLIER SYSTEM
- multiplier = 1 → 5 colors
- multiplier = 3 → 15 colors
- Naming: ROLE_1, ROLE_2, ...
- Iterative expansion: each next column reuses the previous column HIGHLIGHT as the new MAIN (H/S/V preserved).
- Valid range: 1–16

---

# 8. OUTPUT
Text, JSON, or raw HSV.

---

# 8.1 IMAGE_GRAYSCALE_OUTPUT
trigger: --grayscale-image <path>
behavior:
- Load the input image from path.
- Convert each pixel to grayscale using luminance (0.299*R + 0.587*G + 0.114*B).
- Write output image as "image-grayscale-<original-name>" in the same directory.
- Preserve the original file extension.
- If output swatch flag is also provided, both operations may occur (palette output and image grayscale).
- Palette generation still requires a color input unless --random is supplied.

---

# 9. FLAGS
-o, --output-swatch
-s, --square-size <int>
-n, --name <filename>
-O, --swatch-orientation <columns|rows>
-c, --copy
-r, --random
-i, --invert
-g, --grayscale
-G, --grayscale-image <path>
-j, --json
-R, --raw
-C, --config <path>
-W, --no-windows-completions
-m, --multiplier <int>

---

# 10. RANDOM_COLOR_RULES
MAIN.H = random(0–360)
MAIN.S = 70
MAIN.V = random(50–100)

---

# 11. INVERT_RULES
RGB inverted = (255-R, 255-G, 255-B)
Convert to HSV
MAIN.H = inverted.H
MAIN.S = inverted.S
MAIN.V = inverted.V

---

# 12. SWATCH_GENERATION
JPEG output, 5 rows (MAIN, HIGHLIGHT, SHADOW, LIGHT_ACCENT, DARK_ACCENT) and N columns where N=multiplier.
Default square size is 32×32 (configurable) and clamped to a minimum that fits all labels.
Swatch orientation defaults to 5 rows × N columns (N = multiplier). When --swatch-orientation rows is provided, render 5 columns × N rows.
When --grayscale is provided, rows (or columns in rows orientation) are ordered darkest-to-lightest per multiplier column.
Each tile overlay includes:
VARIANT_NAME
HEX: hex(#RRGGBB) (or hex(#RGB) when compressible)
RGB: rgb(R,G,B)
HSV: hsv(H,S,V)

---

# 13. CONFIG_FILE
hsvxp.config.json
Supports: default_square_size, default_multiplier, default_output_name_prefix

---

# 14. ERROR_HANDLING
invalid_color: "ERROR: Invalid color format. Use rgb(), hsv(), hex(), #RGB, or #RRGGBB"
invalid_config: "ERROR: Config file malformed. Using defaults."
swatch_failure: "ERROR: Could not write swatch file."
invalid_swatch_orientation: "ERROR: Invalid swatch orientation. Use columns or rows."
invalid_multiplier: "ERROR: Invalid multiplier. Use 1–16."
missing_color: "ERROR: No color provided. Use a color or --random."
invalid_grayscale_image: "ERROR: Could not process grayscale image."

---

# 15. PERFORMANCE_REQUIREMENTS
- O(1) color math
- minimal allocations
- fast HSV↔RGB conversion
- optimized for AOT

---

# 16. WINDOWS_ONLY_FEATURES
- autocomplete registration
- clipboard integration
- ANSI support detection

---

# 17. SAMPLE_OUTPUT
NAME              H    S    V    R    G    B  HEX
--------------  ---  ---  ---  ---  ---  ---  -------
MAIN_1          357   88   94  240   30   40  #F01E28
HIGHLIGHT_1       2   95  100  255   23   14  #FF170E
SHADOW_1        352   73   67  171   47   63  #AB2F3F
LIGHT_ACCENT_1    7   73  100  255   92   70  #FF5C46
DARK_ACCENT_1   342   53   49  125   59   79  #7D3B4F

---

# 18. SAMPLE_HELP_SCREEN
hsvxp <color> [options]
-o, --output-swatch
-s, --square-size <int>
-n, --name <filename>
-O, --swatch-orientation <columns|rows>
-c, --copy
-r, --random
-i, --invert
-g, --grayscale
-G, --grayscale-image <path>
-j, --json
-R, --raw
-C, --config <path>
-W, --no-windows-completions
-m, --multiplier <int>

