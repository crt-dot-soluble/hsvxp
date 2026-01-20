namespace Hsvxp.Core;

public static class PaletteTextFormatter
{
    public static string FormatLine(PaletteColor color)
    {
        var h = RoundInt(color.Hsv.H);
        var s = RoundInt(color.Hsv.S);
        var v = RoundInt(color.Hsv.V);
        return $"{color.Name.PadRight(15)} H={h} S={s} V={v}   RGB({color.Rgb.R},{color.Rgb.G},{color.Rgb.B})   {color.Hex}";
    }

    private static int RoundInt(double value)
    {
        return (int)Math.Round(value, MidpointRounding.AwayFromZero);
    }
}
