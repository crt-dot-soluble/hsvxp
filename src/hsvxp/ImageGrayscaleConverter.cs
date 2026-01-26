using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Hsvxp;

public static class ImageGrayscaleConverter
{
    public static bool TryConvert(string? inputPath, out string outputPath)
    {
        outputPath = string.Empty;
        if (string.IsNullOrWhiteSpace(inputPath))
        {
            return false;
        }

        try
        {
            if (!File.Exists(inputPath))
            {
                return false;
            }

            var directory = Path.GetDirectoryName(inputPath);
            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Directory.GetCurrentDirectory();
            }

            var fileName = Path.GetFileName(inputPath);
            outputPath = Path.Combine(directory, $"image-grayscale-{fileName}");

            using var image = Image.Load<Rgba32>(inputPath);
            image.ProcessPixelRows(accessor =>
            {
                for (var y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (var x = 0; x < row.Length; x++)
                    {
                        var pixel = row[x];
                        var luminance = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;
                        var gray = (byte)Math.Clamp((int)Math.Round(luminance, MidpointRounding.AwayFromZero), 0, 255);
                        row[x] = new Rgba32(gray, gray, gray, pixel.A);
                    }
                }
            });

            image.Save(outputPath);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
