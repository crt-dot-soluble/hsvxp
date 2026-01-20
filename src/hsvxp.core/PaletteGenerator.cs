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

        foreach (var (role, hOffset, sOffset, vOffset) in Roles)
        {
            var adjusted = ColorMath.ClampHsv(new HsvColor(
                main.H + hOffset,
                main.S + sOffset,
                main.V + vOffset));

            var rgb = ColorMath.HsvToRgb(adjusted);
            var hex = ColorMath.ToHex(rgb);

            for (var index = 1; index <= multiplier; index++)
            {
                var name = $"{role}_{index}";
                results.Add(new PaletteColor(name, adjusted, rgb, hex));
            }
        }

        return results;
    }
}
