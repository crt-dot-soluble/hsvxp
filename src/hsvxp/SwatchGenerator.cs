using Hsvxp.Core;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Hsvxp;

public static class SwatchGenerator
{
    public static void Generate(string filePath, IReadOnlyList<PaletteColor> colors, int squareSize)
    {
        if (squareSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(squareSize));
        }

        var width = squareSize;
        var height = squareSize * colors.Count;

        using var image = new Image<Rgba32>(width, height);
        image.Mutate(ctx => ctx.Fill(Color.Black));

        var font = ResolveFont(squareSize);
        var lineHeight = font.Size + 2;

        for (var index = 0; index < colors.Count; index++)
        {
            var color = colors[index];
            var yOffset = index * squareSize;
            var rect = new Rectangle(0, yOffset, squareSize, squareSize);
            var fillColor = new Rgba32(color.Rgb.R, color.Rgb.G, color.Rgb.B);

            image.Mutate(ctx => ctx.Fill(fillColor, rect));

            var textColor = ChooseTextColor(color.Rgb);
            var startY = yOffset + 4;

            DrawText(image, color.Name, font, textColor, 4, startY);
            DrawText(image, $"HEX: {color.Hex}", font, textColor, 4, startY + lineHeight);
            DrawText(image, $"RGB: {color.Rgb.R},{color.Rgb.G},{color.Rgb.B}", font, textColor, 4, startY + lineHeight * 2);

            var h = (int)Math.Round(color.Hsv.H, MidpointRounding.AwayFromZero);
            var s = (int)Math.Round(color.Hsv.S, MidpointRounding.AwayFromZero);
            var v = (int)Math.Round(color.Hsv.V, MidpointRounding.AwayFromZero);
            DrawText(image, $"HSV: {h},{s},{v}", font, textColor, 4, startY + lineHeight * 3);
        }

        image.SaveAsJpeg(filePath);
    }

    private static void DrawText(Image<Rgba32> image, string text, Font font, Color color, float x, float y)
    {
        image.Mutate(ctx => ctx.DrawText(text, font, color, new PointF(x, y)));
    }

    private static Font ResolveFont(int squareSize)
    {
        var fontSize = Math.Max(10, squareSize / 6f);

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
}
