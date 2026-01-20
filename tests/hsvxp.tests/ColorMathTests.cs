using Hsvxp.Core;

namespace Hsvxp.Tests;

public class ColorMathTests
{
    [Fact]
    public void ConvertsRgbToHsv()
    {
        var hsv = ColorMath.RgbToHsv(new RgbColor(255, 0, 0));

        Assert.Equal(0, Math.Round(hsv.H));
        Assert.Equal(100, Math.Round(hsv.S));
        Assert.Equal(100, Math.Round(hsv.V));
    }

    [Fact]
    public void ConvertsHsvToRgb()
    {
        var rgb = ColorMath.HsvToRgb(new HsvColor(120, 100, 100));

        Assert.Equal((byte)0, rgb.R);
        Assert.Equal((byte)255, rgb.G);
        Assert.Equal((byte)0, rgb.B);
    }

    [Fact]
    public void WrapsHueCorrectly()
    {
        Assert.Equal(350, ColorMath.WrapHue(-10));
        Assert.Equal(10, ColorMath.WrapHue(370));
        Assert.Equal(360, ColorMath.WrapHue(720));
    }

    [Fact]
    public void ClampsHsvValues()
    {
        var clamped = ColorMath.ClampHsv(new HsvColor(370, -5, 120));

        Assert.Equal(10, clamped.H);
        Assert.Equal(0, clamped.S);
        Assert.Equal(100, clamped.V);
    }
}
