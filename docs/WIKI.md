# HSVXP Wiki

## What it does
HSVXP generates deterministic HSV-based palettes for pixel-art workflows. It accepts strict color inputs, applies standardized role offsets, and expands palettes with a multiplier system.

## Grayscale mode
Use `--grayscale` (or `-g`) to force neutral palettes for texture workflows:
- H and S are forced to 0 for all roles.
- Only V offsets are applied.
- Output and swatch rows are ordered darkest-to-lightest per multiplier column.

## Grayscale image conversion
Use `--grayscale-image <path>` (or `-G`) to convert an input image to grayscale for downstream colorization. The output file is written as image-grayscale-<original-name> in the same directory.

## Palette multiplier
The multiplier repeats the five-role palette across columns (1–16). Each new column reuses the previous column HIGHLIGHT as the next MAIN for consistent progression.

## Swatch output
Swatches render as 5×N tiles by default (rows = roles, columns = multiplier), with label overlays for name, hex, rgb, and hsv. Use `--swatch-orientation rows` to swap layout.

## Gallery
Grayscale (multiplier 4):
![Grayscale swatch](swatch-grayscale-128.jpg)

Soft sage (rgb(143,191,166), multiplier 4):
![Sage swatch](swatch-sage-143-191-166.jpg)

Warm sunset (rgb(242,166,90), multiplier 4):
![Sunset swatch](swatch-sunset-242-166-90.jpg)
