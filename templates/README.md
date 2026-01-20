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

## Distribution
Release artifacts are placed in `dist/` after publishing:
- `dist/hsvxp.exe`
- `dist/hsvxp.core.dll`

## Usage
```bash
hsvxp <color> [options]
```

Options:
- `-o, --output-swatch`
- `-s, --square-size <int>`
- `-n, --name <filename>`
- `-O, --swatch-orientation <columns|rows>`
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
Text output (table format):
```text
NAME              H    S    V    R    G    B  HEX
--------------  ---  ---  ---  ---  ---  ---  -------
MAIN_1          357   70   94  240   72   80  #F04850
HIGHLIGHT_1       2   77  100  255   66   59  #FF423B
SHADOW_1        352   55   67  171   77   89  #AB4D59
LIGHT_ACCENT_1    7   55  100  255  131  115  #FF8373
DARK_ACCENT_1   342   35   49  125   81   94  #7D515E

MAIN_2            2   70  100  255   83   77  #FF534D
HIGHLIGHT_2       7   77  100  255   82   59  #FF523B
SHADOW_2        357   55   73  186   84   89  #BA5459
LIGHT_ACCENT_2   12   55  100  255  143  115  #FF8F73
DARK_ACCENT_2   347   35   55  140   91  102  #8C5B66

MAIN_3            7   70  100  255   98   77  #FF624D
HIGHLIGHT_3      12   77  100  255   98   59  #FF623B
SHADOW_3          2   55   73  186   87   84  #BA5754
LIGHT_ACCENT_3   17   55  100  255  155  115  #FF9B73
DARK_ACCENT_3   352   35   55  140   91   98  #8C5B62
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
	"default_square_size": 32,
	"default_multiplier": 1,
	"default_output_name_prefix": "hsvxp"
}
```

## Swatch output
Use `-o` to generate a 5×N swatch grid (5 rows × multiplier columns). Each tile includes the variant name, hex, `rgb(...)`, and `hsv(...)` values. The tile size is clamped to a minimum that fits all labels. Use `--swatch-orientation rows` to swap rows/columns (5 columns × multiplier rows).

![Sample swatch](docs/sample-swatch.jpg)
