using Hsvxp.Core;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Hsvxp;

public static class SwatchGenerator
{
    private static readonly Dictionary<string, int> RoleOrder = new(StringComparer.OrdinalIgnoreCase)
    {
        ["MAIN"] = 0,
        ["HIGHLIGHT"] = 1,
        ["SHADOW"] = 2,
        ["LIGHT_ACCENT"] = 3,
        ["DARK_ACCENT"] = 4
    };

    public static void Generate(string filePath, IReadOnlyList<PaletteColor> colors, int squareSize, SwatchOrientation orientation)
    {
        if (squareSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(squareSize));
        }

        var paletteCount = Math.Max(1, colors.Count / 5);
        var columns = orientation == SwatchOrientation.Rows ? 5 : paletteCount;
        var rows = orientation == SwatchOrientation.Rows ? paletteCount : 5;
        var tileSize = Math.Max(squareSize, CalculateMinimumTileSize(colors, squareSize));
        var width = tileSize * columns;
        var height = tileSize * rows;

        using var image = new Image<Rgba32>(width, height);
        image.Mutate(ctx => ctx.Fill(Color.Black));

        var font = ResolveFont(tileSize);
        var lineHeight = font.Size + 2;
        var grayscaleOrder = BuildGrayscaleOrder(colors);

        foreach (var color in colors)
        {
            if (!TryGetTilePosition(color.Name, orientation, grayscaleOrder, out var column, out var row))
            {
                continue;
            }

            var xOffset = column * tileSize;
            var yOffset = row * tileSize;
            var rect = new Rectangle(xOffset, yOffset, tileSize, tileSize);
            var fillColor = new Rgba32(color.Rgb.R, color.Rgb.G, color.Rgb.B);

            image.Mutate(ctx => ctx.Fill(fillColor, rect));

            var textColor = ChooseTextColor(color.Rgb);
            var startX = xOffset + 6;
            var startY = yOffset + 6;

            DrawText(image, color.Name, font, textColor, startX, startY);
            DrawText(image, $"HEX: {FormatHexInput(color.Hex)}", font, textColor, startX, startY + lineHeight);
            DrawText(image, $"RGB: rgb({color.Rgb.R},{color.Rgb.G},{color.Rgb.B})", font, textColor, startX, startY + lineHeight * 2);

            var h = (int)Math.Round(color.Hsv.H, MidpointRounding.AwayFromZero);
            var s = (int)Math.Round(color.Hsv.S, MidpointRounding.AwayFromZero);
            var v = (int)Math.Round(color.Hsv.V, MidpointRounding.AwayFromZero);
            DrawText(image, $"HSV: hsv({h},{s},{v})", font, textColor, startX, startY + lineHeight * 3);
        }

        image.SaveAsJpeg(filePath);
    }

    private static void DrawText(Image<Rgba32> image, string text, Font font, Color color, float x, float y)
    {
        image.Mutate(ctx => ctx.DrawText(text, font, color, new PointF(x, y)));
    }

    private static Font ResolveFont(int squareSize)
    {
        var maxFontSize = Math.Max(12, (squareSize - 12) / 4f);
        var fontSize = Math.Clamp(maxFontSize, 12, 28);

        if (SystemFonts.TryGet("Segoe UI", out var segui))
        {
            return segui.CreateFont(fontSize, FontStyle.Regular);
        }

        if (SystemFonts.TryGet("Arial", out var arial))
        {
            return arial.CreateFont(fontSize, FontStyle.Regular);
        }

        var family = SystemFonts.Collection.Families.First();
        return family.CreateFont(fontSize, FontStyle.Regular);
    }

    private static Color ChooseTextColor(RgbColor rgb)
    {
        var luminance = (0.299 * rgb.R + 0.587 * rgb.G + 0.114 * rgb.B) / 255.0;
        return luminance > 0.6 ? Color.Black : Color.White;
    }

    private static bool TryGetTilePosition(string name, SwatchOrientation orientation, Dictionary<string, Dictionary<int, int>>? grayscaleOrder, out int column, out int row)
    {
        column = 0;
        row = 0;

        if (!TryParseRoleIndex(name, out var role, out var index))
        {
            return false;
        }

        if (!RoleOrder.TryGetValue(role, out var roleIndex))
        {
            return false;
        }

        var orderedIndex = roleIndex;
        if (grayscaleOrder is not null
            && grayscaleOrder.TryGetValue(role, out var indexMap)
            && indexMap.TryGetValue(index, out var grayscaleIndex))
        {
            orderedIndex = grayscaleIndex;
        }

        var paletteIndex = index - 1;
        if (orientation == SwatchOrientation.Rows)
        {
            column = orderedIndex;
            row = paletteIndex;
            return true;
        }

        column = paletteIndex;
        row = orderedIndex;
        return true;
    }

    private static bool TryParseRoleIndex(string name, out string role, out int index)
    {
        role = string.Empty;
        index = 0;

        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var splitIndex = name.LastIndexOf('_');
        if (splitIndex <= 0 || splitIndex == name.Length - 1)
        {
            return false;
        }

        role = name[..splitIndex];
        var indexText = name[(splitIndex + 1)..];

        return int.TryParse(indexText, out index) && index >= 1;
    }

    private static Dictionary<string, Dictionary<int, int>>? BuildGrayscaleOrder(IReadOnlyList<PaletteColor> colors)
    {
        if (!IsGrayscalePalette(colors))
        {
            return null;
        }

        var byIndex = new Dictionary<int, List<(string Role, double Value)>>();
        foreach (var color in colors)
        {
            if (!TryParseRoleIndex(color.Name, out var role, out var index))
            {
                continue;
            }

            if (!byIndex.TryGetValue(index, out var list))
            {
                list = new List<(string, double)>();
                byIndex[index] = list;
            }

            list.Add((role, color.Hsv.V));
        }

        var order = new Dictionary<string, Dictionary<int, int>>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in byIndex)
        {
            var index = entry.Key;
            var ordered = entry.Value
                .OrderBy(item => item.Value)
                .ThenBy(item => GetRoleOrder(item.Role))
                .ToList();

            for (var rowIndex = 0; rowIndex < ordered.Count; rowIndex++)
            {
                var role = ordered[rowIndex].Role;
                if (!order.TryGetValue(role, out var indexMap))
                {
                    indexMap = new Dictionary<int, int>();
                    order[role] = indexMap;
                }

                indexMap[index] = rowIndex;
            }
        }

        return order;
    }

    private static bool IsGrayscalePalette(IReadOnlyList<PaletteColor> colors)
    {
        if (colors.Count == 0)
        {
            return false;
        }

        const double epsilon = 0.0001;
        return colors.All(color => Math.Abs(color.Hsv.S) <= epsilon);
    }

    private static int GetRoleOrder(string role)
    {
        return RoleOrder.TryGetValue(role, out var index) ? index : int.MaxValue;
    }

    private static int CalculateMinimumTileSize(IReadOnlyList<PaletteColor> colors, int requestedSize)
    {
        var candidate = Math.Max(requestedSize, 32);
        for (var i = 0; i < 3; i++)
        {
            var font = ResolveFont(candidate);
            var required = MeasureRequiredTileSize(colors, font);
            if (required <= candidate)
            {
                return candidate;
            }

            candidate = required;
        }

        return candidate;
    }

    private static int MeasureRequiredTileSize(IReadOnlyList<PaletteColor> colors, Font font)
    {
        var maxNameLength = colors.Any() ? colors.Max(c => c.Name.Length) : 6;
        var sampleName = new string('W', Math.Max(6, maxNameLength));
        var sampleHex = "HEX: hex(#FFFFFF)";
        var sampleRgb = "RGB: rgb(255,255,255)";
        var sampleHsv = "HSV: hsv(360,100,100)";

        var options = new SixLabors.Fonts.TextOptions(font);
        var nameSize = SixLabors.Fonts.TextMeasurer.MeasureSize(sampleName, options);
        var hexSize = SixLabors.Fonts.TextMeasurer.MeasureSize(sampleHex, options);
        var rgbSize = SixLabors.Fonts.TextMeasurer.MeasureSize(sampleRgb, options);
        var hsvSize = SixLabors.Fonts.TextMeasurer.MeasureSize(sampleHsv, options);

        var maxWidth = new[] { nameSize.Width, hexSize.Width, rgbSize.Width, hsvSize.Width }.Max();

        var padding = 12;
        var lineHeight = font.Size + 2;
        var requiredHeight = padding + (lineHeight * 4) + padding;
        var requiredWidth = maxWidth + padding;

        return (int)Math.Ceiling(Math.Max(requiredHeight, requiredWidth));
    }

    private static string FormatHexInput(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex) || hex.Length != 7 || hex[0] != '#')
        {
            return $"hex({hex})";
        }

        var r1 = hex[1];
        var r2 = hex[2];
        var g1 = hex[3];
        var g2 = hex[4];
        var b1 = hex[5];
        var b2 = hex[6];

        if (r1 == r2 && g1 == g2 && b1 == b2)
        {
            return $"hex(#{r1}{g1}{b1})";
        }

        return $"hex({hex})";
    }
}

public enum SwatchOrientation
{
    Columns,
    Rows
}
