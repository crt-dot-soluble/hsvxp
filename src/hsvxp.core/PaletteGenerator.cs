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
        return Generate(main, multiplier, false);
    }

    public static IReadOnlyList<PaletteColor> Generate(HsvColor main, int multiplier, bool grayscale)
    {
        var results = new List<PaletteColor>(Roles.Length * multiplier);
        var currentMain = main;

        for (var index = 1; index <= multiplier; index++)
        {
            HsvColor? highlight = null;
            List<(int RoleIndex, PaletteColor Color, double Value)>? columnColors = grayscale
                ? new List<(int, PaletteColor, double)>(Roles.Length)
                : null;

            for (var roleIndex = 0; roleIndex < Roles.Length; roleIndex++)
            {
                var (role, hOffset, sOffset, vOffset) = Roles[roleIndex];
                var adjusted = grayscale
                    ? ColorMath.ClampHsv(new HsvColor(0, 0, currentMain.V + vOffset))
                    : ColorMath.ClampHsv(new HsvColor(
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
                var color = new PaletteColor(name, adjusted, rgb, hex);

                if (grayscale)
                {
                    columnColors!.Add((roleIndex, color, adjusted.V));
                }
                else
                {
                    results.Add(color);
                }
            }

            if (grayscale && columnColors is not null)
            {
                foreach (var item in columnColors.OrderBy(entry => entry.Value).ThenBy(entry => entry.RoleIndex))
                {
                    results.Add(item.Color);
                }
            }

            if (highlight is not null)
            {
                currentMain = highlight.Value;
            }
        }

        return results;
    }
}
