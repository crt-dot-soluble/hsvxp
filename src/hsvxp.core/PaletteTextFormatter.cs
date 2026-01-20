namespace Hsvxp.Core;

public static class PaletteTextFormatter
{
    public static string FormatLine(PaletteColor color)
    {
        var h = RoundInt(color.Hsv.H);
        var s = RoundInt(color.Hsv.S);
        var v = RoundInt(color.Hsv.V);
        return $"{color.Name.PadRight(15)} H={h,3} S={s,3} V={v,3}   RGB({color.Rgb.R,3},{color.Rgb.G,3},{color.Rgb.B,3})   {color.Hex}";
    }

    private static int RoundInt(double value)
    {
        return (int)Math.Round(value, MidpointRounding.AwayFromZero);
    }
}
