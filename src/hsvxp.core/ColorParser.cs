using System.Globalization;
using System.Text.RegularExpressions;

namespace Hsvxp.Core;

public static class ColorParser
{
    private static readonly Regex HexShortOrLong = new(@"^#(?<hex>[0-9a-fA-F]{3}|[0-9a-fA-F]{6})$", RegexOptions.Compiled);
    private static readonly Regex HexWrapped = new(@"^hex\(\s*#(?<hex>[0-9a-fA-F]{3}|[0-9a-fA-F]{6})\s*\)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex RgbPattern = new(@"^rgb\(\s*(?<r>\d{1,3})\s*,\s*(?<g>\d{1,3})\s*,\s*(?<b>\d{1,3})\s*\)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex HsvPattern = new(@"^hsv\(\s*(?<h>\d{1,3})\s*,\s*(?<s>\d{1,3})\s*,\s*(?<v>\d{1,3})\s*\)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool TryParse(string input, out InputColor color)
    {
        color = default!;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var trimmed = input.Trim();

        if (TryParseHex(trimmed, out var rgbFromHex))
        {
            var hsvFromHex = ColorMath.RgbToHsv(rgbFromHex);
            color = new InputColor(hsvFromHex, rgbFromHex);
            return true;
        }

        if (TryParseRgb(trimmed, out var rgb))
        {
            var hsv = ColorMath.RgbToHsv(rgb);
            color = new InputColor(hsv, rgb);
            return true;
        }

        if (TryParseHsv(trimmed, out var hsvInput))
        {
            var rgbFromHsv = ColorMath.HsvToRgb(hsvInput);
            color = new InputColor(hsvInput, rgbFromHsv);
            return true;
        }

        return false;
    }

    private static bool TryParseHex(string input, out RgbColor rgb)
    {
        rgb = default;
        var match = HexShortOrLong.Match(input);
        if (!match.Success)
        {
            match = HexWrapped.Match(input);
        }

        if (!match.Success)
        {
            return false;
        }

        var hex = match.Groups["hex"].Value;
        if (hex.Length == 3)
        {
            hex = string.Concat(hex.Select(ch => string.Create(CultureInfo.InvariantCulture, $"{ch}{ch}")));
        }

        var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        rgb = new RgbColor(r, g, b);
        return true;
    }

    private static bool TryParseRgb(string input, out RgbColor rgb)
    {
        rgb = default;
        var match = RgbPattern.Match(input);
        if (!match.Success)
        {
            return false;
        }

        if (!int.TryParse(match.Groups["r"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var r) ||
            !int.TryParse(match.Groups["g"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var g) ||
            !int.TryParse(match.Groups["b"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var b))
        {
            return false;
        }

        if (!IsInRange(r, 0, 255) || !IsInRange(g, 0, 255) || !IsInRange(b, 0, 255))
        {
            return false;
        }

        rgb = new RgbColor((byte)r, (byte)g, (byte)b);
        return true;
    }

    private static bool TryParseHsv(string input, out HsvColor hsv)
    {
        hsv = default;
        var match = HsvPattern.Match(input);
        if (!match.Success)
        {
            return false;
        }

        if (!int.TryParse(match.Groups["h"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var h) ||
            !int.TryParse(match.Groups["s"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var s) ||
            !int.TryParse(match.Groups["v"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
        {
            return false;
        }

        if (!IsInRange(h, 0, 360) || !IsInRange(s, 0, 100) || !IsInRange(v, 0, 100))
        {
            return false;
        }

        hsv = new HsvColor(h, s, v);
        return true;
    }

    private static bool IsInRange(int value, int min, int max)
    {
        return value >= min && value <= max;
    }
}
