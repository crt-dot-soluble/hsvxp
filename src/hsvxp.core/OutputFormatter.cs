using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Hsvxp.Core;

public static class OutputFormatter
{
    public static string FormatText(IReadOnlyList<PaletteColor> colors)
    {
        var lines = new List<string>(colors.Count);
        foreach (var color in colors)
        {
            var h = RoundInt(color.Hsv.H);
            var s = RoundInt(color.Hsv.S);
            var v = RoundInt(color.Hsv.V);
            var line = $"{color.Name.PadRight(15)} H={h} S={s} V={v}   RGB({color.Rgb.R},{color.Rgb.G},{color.Rgb.B})   {color.Hex}";
            lines.Add(line);
        }

        return string.Join(Environment.NewLine, lines);
    }

    public static string FormatRaw(IReadOnlyList<PaletteColor> colors)
    {
        var lines = new List<string>(colors.Count);
        foreach (var color in colors)
        {
            var h = RoundInt(color.Hsv.H);
            var s = RoundInt(color.Hsv.S);
            var v = RoundInt(color.Hsv.V);
            lines.Add($"{color.Name} {h},{s},{v}");
        }

        return string.Join(Environment.NewLine, lines);
    }

    public static string FormatJson(IReadOnlyList<PaletteColor> colors)
    {
        var payload = colors.Select(color => new
        {
            name = color.Name,
            hsv = new { h = RoundInt(color.Hsv.H), s = RoundInt(color.Hsv.S), v = RoundInt(color.Hsv.V) },
            rgb = new { r = color.Rgb.R, g = color.Rgb.G, b = color.Rgb.B },
            hex = color.Hex
        });

        return JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            WriteIndented = true,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        });
    }

    private static int RoundInt(double value)
    {
        return (int)Math.Round(value, MidpointRounding.AwayFromZero);
    }
}
