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
        var main = new HsvColor(200, 70, 60);
        var palette = PaletteGenerator.Generate(main, 2);

        var main1 = palette.First(p => p.Name == "MAIN_1");
        var main2 = palette.First(p => p.Name == "MAIN_2");

        Assert.NotEqual(main1.Hsv.H, main2.Hsv.H);
        Assert.NotEqual(main1.Hsv.V, main2.Hsv.V);
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
}
