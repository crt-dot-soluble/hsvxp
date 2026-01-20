using System.Runtime.InteropServices;

namespace Hsvxp.Core;

public static class AnsiSupportDetector
{
    public static bool IsAnsiSupported()
    {
        return IsAnsiSupportedInternal(
            () => Console.IsOutputRedirected,
            () => OperatingSystem.IsWindows(),
            () => Environment.OSVersion.Version,
            IsVirtualTerminalProcessingEnabled);
    }

    internal static bool IsAnsiSupportedInternal(
        Func<bool> isOutputRedirected,
        Func<bool> isWindows,
        Func<Version> getOsVersion,
        Func<bool> isVirtualTerminalEnabled)
    {
        if (isOutputRedirected())
        {
            return false;
        }

        if (!isWindows())
        {
            return true;
        }

        var version = getOsVersion();
        if (version.Major < 10)
        {
            return false;
        }

        return isVirtualTerminalEnabled();
    }

    private static bool IsVirtualTerminalProcessingEnabled()
    {
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        var handle = GetStdHandle(StdOutputHandle);
        if (handle == IntPtr.Zero || handle == new IntPtr(-1))
        {
            return false;
        }

        if (!GetConsoleMode(handle, out var mode))
        {
            return false;
        }

        return (mode & EnableVirtualTerminalProcessing) != 0;
    }

    private const int StdOutputHandle = -11;
    private const uint EnableVirtualTerminalProcessing = 0x0004;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
}
