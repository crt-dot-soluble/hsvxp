using Hsvxp.Core;

namespace Hsvxp.Tests;

public class PaletteTextFormatterTests
{
    [Fact]
    public void FormatsSingleLine()
    {
        var color = new PaletteColor("MAIN_1", new HsvColor(10.4, 70.2, 80.6), new RgbColor(1, 2, 3), "#010203");

        var output = PaletteTextFormatter.FormatLine(color);

        Assert.Equal("MAIN_1          H=10 S=70 V=81   RGB(1,2,3)   #010203", output);
    }
}
