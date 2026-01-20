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

        var parameters = new object?[] { "LIGHT_ACCENT_2", 0, 0 };
        var result = (bool)method!.Invoke(null, parameters)!;

        Assert.True(result);
        Assert.Equal(1, parameters[1]);
        Assert.Equal(3, parameters[2]);
    }
}
