# SPEC_VERSION: 1.0.0
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
- MAIN.S = 70
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
  S: 70
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

# 6. CLAMPING
hue wraps at 0/360.
saturation and value clamp to 0–100.

---

# 7. MULTIPLIER SYSTEM
- multiplier = 1 → 5 colors
- multiplier = 3 → 15 colors
- Naming: ROLE_1, ROLE_2, ...
- Does not alter HSV math
- Valid range: 1–16

---

# 8. OUTPUT
Text, JSON, or raw HSV.

---

# 9. FLAGS
-o, --output-swatch
-s, --square-size <int>
-n, --name <filename>
-c, --copy
-r, --random
-i, --invert
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
MAIN.S = 70
MAIN.H = inverted.H
MAIN.V = inverted.V

---

# 12. SWATCH_GENERATION
JPEG output, vertical layout, 5 × multiplier tiles.
Each tile overlay includes:
VARIANT_NAME
HEX: #RRGGBB
RGB: R,G,B
HSV: H,S,V

---

# 13. CONFIG_FILE
hsvxp.config.json
Supports: default_square_size, default_multiplier, default_output_name_prefix

---

# 14. ERROR_HANDLING
invalid_color: "ERROR: Invalid color format. Use rgb(), hsv(), hex(), #RGB, or #RRGGBB"
invalid_config: "ERROR: Config file malformed. Using defaults."
swatch_failure: "ERROR: Could not write swatch file."
invalid_multiplier: "ERROR: Invalid multiplier. Use 1–16."
missing_color: "ERROR: No color provided. Use a color or --random."

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
MAIN_1          H=355 S=70 V=95   RGB(240,30,40)   #F01E28
HIGHLIGHT_1     H=360 S=77 V=100  RGB(255,60,70)   #FF3C46
SHADOW_1        H=350 S=55 V=68   RGB(175,40,55)   #AF2837
LIGHT_ACCENT_1  H=5   S=55 V=100  RGB(255,150,160) #FF96A0
DARK_ACCENT_1   H=340 S=35 V=50   RGB(130,50,70)   #823246

---

# 18. SAMPLE_HELP_SCREEN
hsvxp <color> [options]
-o, --output-swatch
-s, --square-size <int>
-n, --name <filename>
-c, --copy
-r, --random
-i, --invert
-j, --json
-R, --raw
-C, --config <path>
-W, --no-windows-completions
-m, --multiplier <int>

