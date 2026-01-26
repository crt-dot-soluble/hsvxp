using Hsvxp.Core;

namespace Hsvxp.Tests;

public class PaletteGeneratorTests
{
    [Fact]
    public void GeneratesMultiplierCount()
    {
        var main = new HsvColor(355, 70, 95);
        var palette = PaletteGenerator.Generate(main, 3);

        Assert.Equal(15, palette.Count);
        Assert.Equal("MAIN_1", palette[0].Name);
        Assert.Equal("HIGHLIGHT_1", palette[1].Name);
        Assert.Equal("SHADOW_1", palette[2].Name);
        Assert.Equal("LIGHT_ACCENT_1", palette[3].Name);
        Assert.Equal("DARK_ACCENT_1", palette[4].Name);
        Assert.Equal("MAIN_2", palette[5].Name);
    }

    [Fact]
    public void MultiplierIterationsShiftMain()
    {
        var main = new HsvColor(200, 40, 60);
        var palette = PaletteGenerator.Generate(main, 2);

        var highlight1 = palette.First(p => p.Name == "HIGHLIGHT_1");
        var main2 = palette.First(p => p.Name == "MAIN_2");

        Assert.Equal(highlight1.Hsv.H, main2.Hsv.H);
        Assert.Equal(highlight1.Hsv.S, main2.Hsv.S);
        Assert.Equal(highlight1.Hsv.V, main2.Hsv.V);
    }

    [Fact]
    public void AppliesHighlightOffsets()
    {
        var main = new HsvColor(355, 70, 95);
        var palette = PaletteGenerator.Generate(main, 1);
        var highlight = palette.First(p => p.Name == "HIGHLIGHT_1");

        Assert.Equal(360, highlight.Hsv.H);
        Assert.Equal(77, highlight.Hsv.S);
        Assert.Equal(100, highlight.Hsv.V);
    }

    [Fact]
    public void GrayscaleForcesZeroHueSaturationAndOrdersByValue()
    {
        var main = new HsvColor(210, 55, 60);
        var palette = PaletteGenerator.Generate(main, 1, true);

        Assert.All(palette, color =>
        {
            Assert.Equal(0, color.Hsv.H);
            Assert.Equal(0, color.Hsv.S);
        });

        var expectedOrder = new[]
        {
            "DARK_ACCENT_1",
            "SHADOW_1",
            "MAIN_1",
            "HIGHLIGHT_1",
            "LIGHT_ACCENT_1"
        };

        Assert.Equal(expectedOrder, palette.Select(color => color.Name).ToArray());
    }

    [Fact]
    public void GrayscaleMultiplierUsesHighlightAsNextMain()
    {
        var main = new HsvColor(210, 55, 60);
        var palette = PaletteGenerator.Generate(main, 2, true);

        var highlight1 = palette.First(p => p.Name == "HIGHLIGHT_1");
        var main2 = palette.First(p => p.Name == "MAIN_2");

        Assert.Equal(highlight1.Hsv.H, main2.Hsv.H);
        Assert.Equal(highlight1.Hsv.S, main2.Hsv.S);
        Assert.Equal(highlight1.Hsv.V, main2.Hsv.V);
    }
}
