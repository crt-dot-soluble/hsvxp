# HSVXP

Deterministic HSV-based palette expansion CLI for pixel-art workflows. HSVXP accepts strict color inputs, generates a five-role palette, supports multiplier expansion, and can emit text, JSON, raw HSV, or JPEG swatch output.

## Features
- Strict color parsing: `#RGB`, `#RRGGBB`, `hex(#RGB)`, `hex(#RRGGBB)`, `rgb(R,G,B)`, `hsv(H,S,V)`
- HSV normalization with standardized palette roles
- Multiplier expansion from 1–16
- Text, JSON, or raw HSV output
- JPEG swatch generation with labeled tiles
- Config file support via `hsvxp.config.json`
- Windows-only clipboard copy and PowerShell completion registration

## Requirements
- .NET (latest LTS)

## Build
```bash
dotnet build hsvxp.sln
```

## Test
```bash
dotnet test hsvxp.sln
```

## Usage
```bash
hsvxp <color> [options]
```

Options:
- `-o, --output-swatch`
- `-s, --square-size <int>`
- `-n, --name <filename>`
- `-c, --copy`
- `-r, --random`
- `-i, --invert`
- `-j, --json`
- `-R, --raw`
- `-C, --config <path>`
- `-W, --no-windows-completions`
- `-m, --multiplier <int>`

## Input formats
Accepted formats (case-insensitive, whitespace tolerant):
- `#RRGGBB`
- `#RGB`
- `hex(#RRGGBB)`
- `hex(#RGB)`
- `rgb(R,G,B)`
- `hsv(H,S,V)`

## Example output
Default text output:
```text
MAIN_1          H=357 S=70 V=94   RGB(240,72,80)   #F04850
HIGHLIGHT_1     H=2 S=77 V=100   RGB(255,66,59)   #FF423B
SHADOW_1        H=352 S=55 V=67   RGB(171,77,89)   #AB4D59
LIGHT_ACCENT_1  H=7 S=55 V=100   RGB(255,131,115)   #FF8373
DARK_ACCENT_1   H=342 S=35 V=49   RGB(125,81,94)   #7D515E
```

JSON output:
```json
[
  {
    "name": "MAIN_1",
    "hsv": { "h": 357, "s": 70, "v": 94 },
    "rgb": { "r": 240, "g": 72, "b": 80 },
    "hex": "#F04850"
  },
  {
    "name": "HIGHLIGHT_1",
    "hsv": { "h": 2, "s": 77, "v": 100 },
    "rgb": { "r": 255, "g": 66, "b": 59 },
    "hex": "#FF423B"
  },
  {
    "name": "SHADOW_1",
    "hsv": { "h": 352, "s": 55, "v": 67 },
    "rgb": { "r": 171, "g": 77, "b": 89 },
    "hex": "#AB4D59"
  },
  {
    "name": "LIGHT_ACCENT_1",
    "hsv": { "h": 7, "s": 55, "v": 100 },
    "rgb": { "r": 255, "g": 131, "b": 115 },
    "hex": "#FF8373"
  },
  {
    "name": "DARK_ACCENT_1",
    "hsv": { "h": 342, "s": 35, "v": 49 },
    "rgb": { "r": 125, "g": 81, "b": 94 },
    "hex": "#7D515E"
  }
]
```

Raw HSV output:
```text
MAIN_1 357,70,94
HIGHLIGHT_1 2,77,100
SHADOW_1 352,55,67
LIGHT_ACCENT_1 7,55,100
DARK_ACCENT_1 342,35,49
```

## Config file
Create `hsvxp.config.json`:
```json
{
  "default_square_size": 512,
  "default_multiplier": 1,
  "default_output_name_prefix": "hsvxp"
}
```

## Swatch output
Use `-o` to generate a vertical JPEG swatch. Each tile includes the variant name, hex, RGB, and HSV values.
