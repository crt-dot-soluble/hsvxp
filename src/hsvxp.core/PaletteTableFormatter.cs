using System.Text;

namespace Hsvxp.Core;

public static class PaletteTableFormatter
{
    public static string Format(IReadOnlyList<PaletteColor> colors)
    {
        var nameWidth = Math.Max(4, colors.Any() ? colors.Max(c => c.Name.Length) : 4);
        var header = new[]
        {
            ("NAME", nameWidth, true),
            ("H", 3, false),
            ("S", 3, false),
            ("V", 3, false),
            ("R", 3, false),
            ("G", 3, false),
            ("B", 3, false),
            ("HEX", 7, true)
        };

        var builder = new StringBuilder();
        builder.AppendLine(BuildRow(header, header.Select(h => h.Item1).ToArray()));
        builder.AppendLine(BuildUnderline(header));

        var lastGroup = -1;
        foreach (var color in colors)
        {
            var groupIndex = GetGroupIndex(color.Name);
            if (lastGroup != -1 && groupIndex != -1 && groupIndex != lastGroup)
            {
                builder.AppendLine();
            }

            var h = RoundInt(color.Hsv.H);
            var s = RoundInt(color.Hsv.S);
            var v = RoundInt(color.Hsv.V);
            var row = new[]
            {
                color.Name,
                h.ToString(),
                s.ToString(),
                v.ToString(),
                color.Rgb.R.ToString(),
                color.Rgb.G.ToString(),
                color.Rgb.B.ToString(),
                color.Hex
            };
            builder.AppendLine(BuildRow(header, row));
            if (groupIndex != -1)
            {
                lastGroup = groupIndex;
            }
        }

        return builder.ToString().TrimEnd();
    }

    private static string BuildRow((string, int, bool)[] columns, string[] values)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < columns.Length; i++)
        {
            var (_, width, leftAlign) = columns[i];
            var value = values[i];
            var padded = leftAlign
                ? value.PadRight(width)
                : value.PadLeft(width);
            if (i > 0)
            {
                builder.Append("  ");
            }
            builder.Append(padded);
        }

        return builder.ToString();
    }

    private static string BuildUnderline((string, int, bool)[] columns)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < columns.Length; i++)
        {
            var (_, width, _) = columns[i];
            if (i > 0)
            {
                builder.Append("  ");
            }
            builder.Append(new string('-', width));
        }

        return builder.ToString();
    }

    private static int RoundInt(double value)
    {
        return (int)Math.Round(value, MidpointRounding.AwayFromZero);
    }

    private static int GetGroupIndex(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return -1;
        }

        var splitIndex = name.LastIndexOf('_');
        if (splitIndex <= 0 || splitIndex == name.Length - 1)
        {
            return -1;
        }

        var indexText = name[(splitIndex + 1)..];
        return int.TryParse(indexText, out var index) ? index : -1;
    }
}
