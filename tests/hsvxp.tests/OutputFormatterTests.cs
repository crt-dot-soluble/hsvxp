using System.Text.Json;
using Hsvxp.Core;

namespace Hsvxp.Tests;

public class OutputFormatterTests
{
    [Fact]
    public void FormatsTextOutput()
    {
        var palette = new List<PaletteColor>
        {
            new("MAIN_1", new HsvColor(10.4, 70.2, 80.6), new RgbColor(1, 2, 3), "#010203"),
            new("MAIN_2", new HsvColor(20.4, 70.2, 80.6), new RgbColor(4, 5, 6), "#040506")
        };

        var output = OutputFormatter.FormatText(palette);
        var lines = output.Split(Environment.NewLine);
        Assert.True(lines.Length >= 4);
        Assert.Contains("NAME", lines[0]);
        Assert.Contains("HEX", lines[0]);
        Assert.True(lines[1].All(ch => ch == '-' || ch == ' '));
        Assert.Contains("MAIN_1", output);
        Assert.Contains("MAIN_2", output);
        Assert.Contains(Environment.NewLine + Environment.NewLine, output);
    }

    [Fact]
    public void FormatsRawOutput()
    {
        var palette = new List<PaletteColor>
        {
            new("MAIN_1", new HsvColor(10.4, 70.2, 80.6), new RgbColor(1, 2, 3), "#010203")
        };

        var output = OutputFormatter.FormatRaw(palette);

        Assert.Equal("MAIN_1 10,70,81", output);
    }

    [Fact]
    public void FormatsJsonOutput()
    {
        var palette = new List<PaletteColor>
        {
            new("MAIN_1", new HsvColor(10.4, 70.2, 80.6), new RgbColor(1, 2, 3), "#010203")
        };

        var output = OutputFormatter.FormatJson(palette);
        using var document = JsonDocument.Parse(output);

        var root = document.RootElement;
        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        var item = root[0];
        Assert.Equal("MAIN_1", item.GetProperty("name").GetString());
        Assert.Equal(10, item.GetProperty("hsv").GetProperty("h").GetInt32());
        Assert.Equal(70, item.GetProperty("hsv").GetProperty("s").GetInt32());
        Assert.Equal(81, item.GetProperty("hsv").GetProperty("v").GetInt32());
        Assert.Equal(1, item.GetProperty("rgb").GetProperty("r").GetInt32());
        Assert.Equal(2, item.GetProperty("rgb").GetProperty("g").GetInt32());
        Assert.Equal(3, item.GetProperty("rgb").GetProperty("b").GetInt32());
        Assert.Equal("#010203", item.GetProperty("hex").GetString());
    }
}
