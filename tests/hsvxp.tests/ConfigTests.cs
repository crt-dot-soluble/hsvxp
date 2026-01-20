using Hsvxp.Core;

namespace Hsvxp.Tests;

public class ConfigTests
{
    [Fact]
    public void LoadsDefaultsWhenMissing()
    {
        var config = Config.Load(Path.Combine(Path.GetTempPath(), "missing.config.json"), out var invalid);

        Assert.False(invalid);
        Assert.Equal(512, config.DefaultSquareSize);
        Assert.Equal(1, config.DefaultMultiplier);
        Assert.Equal("hsvxp", config.DefaultOutputNamePrefix);
    }

    [Fact]
    public void FlagsInvalidJson()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "{invalid-json");
            var config = Config.Load(path, out var invalid);

            Assert.True(invalid);
            Assert.Equal(512, config.DefaultSquareSize);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadsCustomValues()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "{ \"default_square_size\": 32, \"default_multiplier\": 2, \"default_output_name_prefix\": \"demo\" }");
            var config = Config.Load(path, out var invalid);

            Assert.False(invalid);
            Assert.Equal(32, config.DefaultSquareSize);
            Assert.Equal(2, config.DefaultMultiplier);
            Assert.Equal("demo", config.DefaultOutputNamePrefix);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
