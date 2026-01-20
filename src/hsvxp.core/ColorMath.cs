using System.Globalization;

namespace Hsvxp.Core;

public static class ColorMath
{
    public static HsvColor RgbToHsv(RgbColor rgb)
    {
        var r = rgb.R / 255.0;
        var g = rgb.G / 255.0;
        var b = rgb.B / 255.0;

        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;

        double h;
        if (delta == 0)
        {
            h = 0;
        }
        else if (max == r)
        {
            h = 60 * (((g - b) / delta) % 6);
        }
        else if (max == g)
        {
            h = 60 * (((b - r) / delta) + 2);
        }
        else
        {
            h = 60 * (((r - g) / delta) + 4);
        }

        if (h < 0)
        {
            h += 360;
        }

        var s = max == 0 ? 0 : (delta / max) * 100;
        var v = max * 100;

        return new HsvColor(h, s, v);
    }

    public static RgbColor HsvToRgb(HsvColor hsv)
    {
        var h = hsv.H == 360 ? 0 : hsv.H;
        var s = hsv.S / 100.0;
        var v = hsv.V / 100.0;

        var c = v * s;
        var x = c * (1 - Math.Abs(((h / 60.0) % 2) - 1));
        var m = v - c;

        double r1;
        double g1;
        double b1;

        if (h < 60)
        {
            r1 = c;
            g1 = x;
            b1 = 0;
        }
        else if (h < 120)
        {
            r1 = x;
            g1 = c;
            b1 = 0;
        }
        else if (h < 180)
        {
            r1 = 0;
            g1 = c;
            b1 = x;
        }
        else if (h < 240)
        {
            r1 = 0;
            g1 = x;
            b1 = c;
        }
        else if (h < 300)
        {
            r1 = x;
            g1 = 0;
            b1 = c;
        }
        else
        {
            r1 = c;
            g1 = 0;
            b1 = x;
        }

        var r = (byte)Math.Clamp(Math.Round((r1 + m) * 255), 0, 255);
        var g = (byte)Math.Clamp(Math.Round((g1 + m) * 255), 0, 255);
        var b = (byte)Math.Clamp(Math.Round((b1 + m) * 255), 0, 255);

        return new RgbColor(r, g, b);
    }

    public static HsvColor ClampHsv(HsvColor hsv)
    {
        var h = WrapHue(hsv.H);
        var s = Math.Clamp(hsv.S, 0, 100);
        var v = Math.Clamp(hsv.V, 0, 100);
        return new HsvColor(h, s, v);
    }

    public static double WrapHue(double h)
    {
        if (h >= 0 && h <= 360)
        {
            return h;
        }

        var wrapped = h % 360;
        if (wrapped < 0)
        {
            wrapped += 360;
        }

        if (wrapped == 0 && h > 0)
        {
            return 360;
        }

        return wrapped;
    }

    public static string ToHex(RgbColor rgb)
    {
        return string.Create(CultureInfo.InvariantCulture, $"#{rgb.R:X2}{rgb.G:X2}{rgb.B:X2}");
    }
}
