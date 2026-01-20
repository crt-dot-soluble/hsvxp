namespace Hsvxp.Core;

public readonly record struct RgbColor(byte R, byte G, byte B);

public readonly record struct HsvColor(double H, double S, double V);

public sealed record InputColor(HsvColor Hsv, RgbColor Rgb);

public sealed record PaletteColor(string Name, HsvColor Hsv, RgbColor Rgb, string Hex);
