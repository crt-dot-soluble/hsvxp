using System.Reflection;
using Hsvxp;
using Hsvxp.Core;

namespace Hsvxp.Tests;

public class SwatchGeneratorTests
{
    [Fact]
    public void ParsesAccentRolesWithUnderscore()
    {
        var method = typeof(SwatchGenerator).GetMethod("TryGetTilePosition", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var parameters = new object?[] { "LIGHT_ACCENT_2", SwatchOrientation.Columns, null, 0, 0 };
        var result = (bool)method!.Invoke(null, parameters)!;

        Assert.True(result);
        Assert.Equal(1, parameters[3]);
        Assert.Equal(3, parameters[4]);
    }

    [Fact]
    public void SwapsRowsAndColumnsWhenRowsOrientationSelected()
    {
        var method = typeof(SwatchGenerator).GetMethod("TryGetTilePosition", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var parameters = new object?[] { "LIGHT_ACCENT_2", SwatchOrientation.Rows, null, 0, 0 };
        var result = (bool)method!.Invoke(null, parameters)!;

        Assert.True(result);
        Assert.Equal(3, parameters[3]);
        Assert.Equal(1, parameters[4]);
    }

    [Fact]
    public void OrdersGrayscaleRowsByBrightness()
    {
        var buildMethod = typeof(SwatchGenerator).GetMethod("BuildGrayscaleOrder", BindingFlags.NonPublic | BindingFlags.Static);
        var positionMethod = typeof(SwatchGenerator).GetMethod("TryGetTilePosition", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(buildMethod);
        Assert.NotNull(positionMethod);

        var colors = new List<PaletteColor>
        {
            new("DARK_ACCENT_1", new HsvColor(0, 0, 10), new RgbColor(10, 10, 10), "#0A0A0A"),
            new("SHADOW_1", new HsvColor(0, 0, 30), new RgbColor(30, 30, 30), "#1E1E1E"),
            new("MAIN_1", new HsvColor(0, 0, 60), new RgbColor(60, 60, 60), "#3C3C3C"),
            new("HIGHLIGHT_1", new HsvColor(0, 0, 80), new RgbColor(80, 80, 80), "#505050"),
            new("LIGHT_ACCENT_1", new HsvColor(0, 0, 95), new RgbColor(95, 95, 95), "#5F5F5F")
        };

        var grayscaleOrder = buildMethod!.Invoke(null, new object?[] { colors });

        AssertRow(positionMethod!, "DARK_ACCENT_1", grayscaleOrder, 0);
        AssertRow(positionMethod!, "SHADOW_1", grayscaleOrder, 1);
        AssertRow(positionMethod!, "MAIN_1", grayscaleOrder, 2);
        AssertRow(positionMethod!, "HIGHLIGHT_1", grayscaleOrder, 3);
        AssertRow(positionMethod!, "LIGHT_ACCENT_1", grayscaleOrder, 4);
    }

    private static void AssertRow(MethodInfo positionMethod, string name, object? grayscaleOrder, int expectedRow)
    {
        var parameters = new object?[] { name, SwatchOrientation.Columns, grayscaleOrder, 0, 0 };
        var result = (bool)positionMethod.Invoke(null, parameters)!;

        Assert.True(result);
        Assert.Equal(expectedRow, parameters[4]);
    }
}
