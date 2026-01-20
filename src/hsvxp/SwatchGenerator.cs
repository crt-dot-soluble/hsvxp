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

    public static void Generate(string filePath, IReadOnlyList<PaletteColor> colors, int squareSize)
    {
        if (squareSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(squareSize));
        }

        var columns = Math.Max(1, colors.Count / 5);
        var tileSize = squareSize;
        var width = tileSize * columns;
        var height = tileSize * 5;

        using var image = new Image<Rgba32>(width, height);
        image.Mutate(ctx => ctx.Fill(Color.Black));

        var font = ResolveFont(tileSize);
        var lineHeight = font.Size + 2;

        foreach (var color in colors)
        {
            if (!TryGetTilePosition(color.Name, out var column, out var row))
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
            DrawText(image, $"HEX: {color.Hex}", font, textColor, startX, startY + lineHeight);
            DrawText(image, $"RGB: {color.Rgb.R},{color.Rgb.G},{color.Rgb.B}", font, textColor, startX, startY + lineHeight * 2);

            var h = (int)Math.Round(color.Hsv.H, MidpointRounding.AwayFromZero);
            var s = (int)Math.Round(color.Hsv.S, MidpointRounding.AwayFromZero);
            var v = (int)Math.Round(color.Hsv.V, MidpointRounding.AwayFromZero);
            DrawText(image, $"HSV: {h},{s},{v}", font, textColor, startX, startY + lineHeight * 3);
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

    private static bool TryGetTilePosition(string name, out int column, out int row)
    {
        column = 0;
        row = 0;

        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var splitIndex = name.LastIndexOf('_');
        if (splitIndex <= 0 || splitIndex == name.Length - 1)
        {
            return false;
        }

        var role = name[..splitIndex];
        var indexText = name[(splitIndex + 1)..];

        if (!int.TryParse(indexText, out var index) || index < 1)
        {
            return false;
        }

        if (!RoleOrder.TryGetValue(role, out row))
        {
            return false;
        }

        column = index - 1;
        return true;
    }
}
