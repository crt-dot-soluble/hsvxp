namespace Hsvxp.Core;

public static class PaletteGenerator
{
    private static readonly (string Role, double H, double S, double V)[] Roles =
    [
        ("MAIN", 0, 0, 0),
        ("HIGHLIGHT", 5, 7, 20),
        ("SHADOW", -5, -15, -27),
        ("LIGHT_ACCENT", 10, -15, 35),
        ("DARK_ACCENT", -15, -35, -45)
    ];

    public static IReadOnlyList<PaletteColor> Generate(HsvColor main, int multiplier)
    {
        var results = new List<PaletteColor>(Roles.Length * multiplier);
        var currentMain = main;

        for (var index = 1; index <= multiplier; index++)
        {
            HsvColor? highlight = null;

            foreach (var (role, hOffset, sOffset, vOffset) in Roles)
            {
                var adjusted = ColorMath.ClampHsv(new HsvColor(
                    currentMain.H + hOffset,
                    currentMain.S + sOffset,
                    currentMain.V + vOffset));

                if (role == "HIGHLIGHT")
                {
                    highlight = adjusted;
                }

                var rgb = ColorMath.HsvToRgb(adjusted);
                var hex = ColorMath.ToHex(rgb);
                var name = $"{role}_{index}";
                results.Add(new PaletteColor(name, adjusted, rgb, hex));
            }

            if (highlight is not null)
            {
                currentMain = new HsvColor(highlight.Value.H, 70, highlight.Value.V);
            }
        }

        return results;
    }
}
