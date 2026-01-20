using System.Reflection;
using Hsvxp;

namespace Hsvxp.Tests;

public class SwatchGeneratorTests
{
    [Fact]
    public void ParsesAccentRolesWithUnderscore()
    {
        var method = typeof(SwatchGenerator).GetMethod("TryGetTilePosition", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var parameters = new object?[] { "LIGHT_ACCENT_2", SwatchOrientation.Columns, 0, 0 };
        var result = (bool)method!.Invoke(null, parameters)!;

        Assert.True(result);
        Assert.Equal(1, parameters[2]);
        Assert.Equal(3, parameters[3]);
    }

    [Fact]
    public void SwapsRowsAndColumnsWhenRowsOrientationSelected()
    {
        var method = typeof(SwatchGenerator).GetMethod("TryGetTilePosition", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var parameters = new object?[] { "LIGHT_ACCENT_2", SwatchOrientation.Rows, 0, 0 };
        var result = (bool)method!.Invoke(null, parameters)!;

        Assert.True(result);
        Assert.Equal(3, parameters[2]);
        Assert.Equal(1, parameters[3]);
    }
}
