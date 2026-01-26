using Hsvxp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Hsvxp.Tests;

public class ImageGrayscaleConverterTests
{
    [Fact]
    public void ConvertsImageAndPreservesNamePrefix()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        var inputPath = Path.Combine(tempDir, "sample.png");
        var outputPath = Path.Combine(tempDir, "image-grayscale-sample.png");

        try
        {
            using (var image = new Image<Rgba32>(2, 1))
            {
                image[0, 0] = new Rgba32(100, 150, 200, 255);
                image[1, 0] = new Rgba32(20, 40, 60, 128);
                image.Save(inputPath);
            }

            var result = ImageGrayscaleConverter.TryConvert(inputPath, out var actualOutput);

            Assert.True(result);
            Assert.Equal(outputPath, actualOutput);
            Assert.True(File.Exists(actualOutput));

            using var converted = Image.Load<Rgba32>(actualOutput);
            var pixel1 = converted[0, 0];
            var pixel2 = converted[1, 0];

            Assert.Equal(141, pixel1.R);
            Assert.Equal(141, pixel1.G);
            Assert.Equal(141, pixel1.B);
            Assert.Equal(255, pixel1.A);

            var expected2 = (byte)Math.Round(0.299 * 20 + 0.587 * 40 + 0.114 * 60, MidpointRounding.AwayFromZero);
            Assert.Equal(expected2, pixel2.R);
            Assert.Equal(expected2, pixel2.G);
            Assert.Equal(expected2, pixel2.B);
            Assert.Equal(128, pixel2.A);
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            if (File.Exists(inputPath))
            {
                File.Delete(inputPath);
            }

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
