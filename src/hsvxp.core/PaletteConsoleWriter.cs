namespace Hsvxp.Core;

public static class PaletteConsoleWriter
{
    public static void WriteText(IReadOnlyList<PaletteColor> colors, TextWriter? writer = null)
    {
        var output = OutputFormatter.FormatText(colors);
        (writer ?? Console.Out).WriteLine(output);
    }
}
