namespace Hsvxp.Core;

public static class Errors
{
    public const string InvalidColor = "ERROR: Invalid color format. Use rgb(), hsv(), hex(), #RGB, or #RRGGBB";
    public const string InvalidConfig = "ERROR: Config file malformed. Using defaults.";
    public const string SwatchFailure = "ERROR: Could not write swatch file.";
    public const string InvalidSwatchOrientation = "ERROR: Invalid swatch orientation. Use columns or rows.";
    public const string InvalidMultiplier = "ERROR: Invalid multiplier. Use 1–16.";
    public const string MissingColor = "ERROR: No color provided. Use a color or --random.";
    public const string InvalidGrayscaleImage = "ERROR: Could not process grayscale image.";
}
