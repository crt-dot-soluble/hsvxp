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
        Assert.Equal("MAIN_2", palette[1].Name);
        Assert.Equal("MAIN_3", palette[2].Name);
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
