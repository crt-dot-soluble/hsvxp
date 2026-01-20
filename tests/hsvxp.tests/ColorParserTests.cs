using Hsvxp.Core;

namespace Hsvxp.Tests;

public class ColorParserTests
{
    [Fact]
    public void ParsesShortHex()
    {
        var success = ColorParser.TryParse("#F0A", out var color);

        Assert.True(success);
        Assert.Equal((byte)255, color.Rgb.R);
        Assert.Equal((byte)0, color.Rgb.G);
        Assert.Equal((byte)170, color.Rgb.B);
    }

    [Fact]
    public void ParsesRgbWithSpaces()
    {
        var success = ColorParser.TryParse("rgb(10, 20, 30)", out var color);

        Assert.True(success);
        Assert.Equal((byte)10, color.Rgb.R);
        Assert.Equal((byte)20, color.Rgb.G);
        Assert.Equal((byte)30, color.Rgb.B);
    }

    [Fact]
    public void ParsesHsvInRange()
    {
        var success = ColorParser.TryParse("hsv(360,100,0)", out var color);

        Assert.True(success);
        Assert.Equal(360, color.Hsv.H);
        Assert.Equal(100, color.Hsv.S);
        Assert.Equal(0, color.Hsv.V);
    }

    [Fact]
    public void RejectsInvalidFormat()
    {
        var success = ColorParser.TryParse("123,45,6", out _);

        Assert.False(success);
    }
}
