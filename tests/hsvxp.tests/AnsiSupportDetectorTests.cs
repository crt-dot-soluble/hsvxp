using Hsvxp.Core;

namespace Hsvxp.Tests;

public class AnsiSupportDetectorTests
{
    [Fact]
    public void ReturnsFalseWhenOutputRedirected()
    {
        var result = AnsiSupportDetector.IsAnsiSupportedInternal(
            () => true,
            () => false,
            () => new Version(10, 0),
            () => true);

        Assert.False(result);
    }

    [Fact]
    public void ReturnsTrueOnNonWindows()
    {
        var result = AnsiSupportDetector.IsAnsiSupportedInternal(
            () => false,
            () => false,
            () => new Version(10, 0),
            () => false);

        Assert.True(result);
    }

    [Fact]
    public void ReturnsFalseWhenVirtualTerminalDisabled()
    {
        var result = AnsiSupportDetector.IsAnsiSupportedInternal(
            () => false,
            () => true,
            () => new Version(10, 0),
            () => false);

        Assert.False(result);
    }

    [Fact]
    public void ReturnsTrueWhenVirtualTerminalEnabled()
    {
        var result = AnsiSupportDetector.IsAnsiSupportedInternal(
            () => false,
            () => true,
            () => new Version(10, 0),
            () => true);

        Assert.True(result);
    }
}
