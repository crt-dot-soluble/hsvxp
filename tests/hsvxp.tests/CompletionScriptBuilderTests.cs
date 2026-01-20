using Hsvxp.Core;

namespace Hsvxp.Tests;

public class CompletionScriptBuilderTests
{
    [Fact]
    public void IncludesMarkerAndCommandName()
    {
        var script = CompletionScriptBuilder.BuildPowerShell("hsvxp", new[] { "-o", "--output-swatch" });

        Assert.Contains(CompletionScriptBuilder.Marker, script);
        Assert.Contains("CommandName \"hsvxp\"", script);
    }

    [Fact]
    public void DeduplicatesOptions()
    {
        var script = CompletionScriptBuilder.BuildPowerShell("hsvxp", new[] { "-o", "-o", "--output-swatch" });

        var lines = script.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var occurrences = lines.Count(line => line.Trim() == "\"-o\"");
        Assert.Equal(1, occurrences);
    }
}
