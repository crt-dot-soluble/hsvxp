using System.CommandLine;
using System.CommandLine.Invocation;
using Hsvxp;
using Hsvxp.Core;
using TextCopy;

internal static class Program
{
	private const int ExitFailure = 1;

	public static async Task<int> Main(string[] args)
	{
		var colorArgument = new Argument<string?>("color", () => null)
		{
			Description = "Input color in rgb(), hsv(), hex(), #RGB, or #RRGGBB format."
		};

		var outputSwatchOption = new Option<bool>(new[] { "-o", "--output-swatch" }, "Write JPEG swatch output.");
		var squareSizeOption = new Option<int?>(new[] { "-s", "--square-size" }, "Square size in pixels.");
		var nameOption = new Option<string?>(new[] { "-n", "--name" }, "Swatch filename.");
		var copyOption = new Option<bool>(new[] { "-c", "--copy" }, "Copy output to clipboard (Windows only).");
		var randomOption = new Option<bool>(new[] { "-r", "--random" }, "Generate a random base color.");
		var invertOption = new Option<bool>(new[] { "-i", "--invert" }, "Invert the input RGB color before HSV normalization.");
		var jsonOption = new Option<bool>(new[] { "-j", "--json" }, "Output JSON.");
		var rawOption = new Option<bool>(new[] { "-R", "--raw" }, "Output raw HSV.");
		var configOption = new Option<string?>(new[] { "-C", "--config" }, "Path to hsvxp.config.json.");
		var noWindowsCompletionsOption = new Option<bool>(new[] { "-W", "--no-windows-completions" }, "Skip Windows autocomplete registration.");
		var multiplierOption = new Option<int?>(new[] { "-m", "--multiplier" }, "Palette multiplier 1-16.");

		var rootCommand = new RootCommand("HSVXP palette generator")
		{
			colorArgument,
			outputSwatchOption,
			squareSizeOption,
			nameOption,
			copyOption,
			randomOption,
			invertOption,
			jsonOption,
			rawOption,
			configOption,
			noWindowsCompletionsOption,
			multiplierOption
		};

		rootCommand.SetHandler(context =>
		{
			var parsedColor = context.ParseResult.GetValueForArgument(colorArgument);
			var outputSwatch = context.ParseResult.GetValueForOption(outputSwatchOption);
			var squareSize = context.ParseResult.GetValueForOption(squareSizeOption);
			var name = context.ParseResult.GetValueForOption(nameOption);
			var copy = context.ParseResult.GetValueForOption(copyOption);
			var random = context.ParseResult.GetValueForOption(randomOption);
			var invert = context.ParseResult.GetValueForOption(invertOption);
			var json = context.ParseResult.GetValueForOption(jsonOption);
			var raw = context.ParseResult.GetValueForOption(rawOption);
			var configPath = context.ParseResult.GetValueForOption(configOption);
			var noWindowsCompletions = context.ParseResult.GetValueForOption(noWindowsCompletionsOption);
			var multiplier = context.ParseResult.GetValueForOption(multiplierOption);

			var optionAliases = new[]
			{
				"-o", "--output-swatch",
				"-s", "--square-size",
				"-n", "--name",
				"-c", "--copy",
				"-r", "--random",
				"-i", "--invert",
				"-j", "--json",
				"-R", "--raw",
				"-C", "--config",
				"-W", "--no-windows-completions",
				"-m", "--multiplier"
			};

			if (!noWindowsCompletions)
			{
				WindowsCompletionRegistrar.TryRegister("hsvxp", optionAliases);
			}

			_ = AnsiSupportDetector.IsAnsiSupported();

			var config = Config.Load(configPath, out var invalidConfig);
			if (invalidConfig)
			{
				Console.WriteLine(Errors.InvalidConfig);
			}

			var resolvedMultiplier = multiplier ?? config.DefaultMultiplier;
			if (resolvedMultiplier is < 1 or > 16)
			{
				Console.WriteLine(Errors.InvalidMultiplier);
				context.ExitCode = ExitFailure;
				return;
			}

			var resolvedSquareSize = squareSize ?? config.DefaultSquareSize;

			if (!random && string.IsNullOrWhiteSpace(parsedColor))
			{
				Console.WriteLine(Errors.MissingColor);
				context.ExitCode = ExitFailure;
				return;
			}

			InputColor? inputColor = null;
			if (!random)
			{
				if (!ColorParser.TryParse(parsedColor!, out var parsed))
				{
					Console.WriteLine(Errors.InvalidColor);
					context.ExitCode = ExitFailure;
					return;
				}

				inputColor = parsed;
			}

			var main = random
				? CreateRandomMain()
				: new HsvColor(inputColor!.Hsv.H, 70, inputColor!.Hsv.V);

			if (invert)
			{
				var rgb = random
					? ColorMath.HsvToRgb(main)
					: inputColor!.Rgb;

				var inverted = new RgbColor((byte)(255 - rgb.R), (byte)(255 - rgb.G), (byte)(255 - rgb.B));
				var invertedHsv = ColorMath.RgbToHsv(inverted);
				main = new HsvColor(invertedHsv.H, 70, invertedHsv.V);
			}

			var palette = PaletteGenerator.Generate(main, resolvedMultiplier);
			var output = json
				? OutputFormatter.FormatJson(palette)
				: raw
					? OutputFormatter.FormatRaw(palette)
					: OutputFormatter.FormatText(palette);

			Console.WriteLine(output);

			if (copy && OperatingSystem.IsWindows())
			{
				try
				{
					ClipboardService.SetText(output);
				}
				catch
				{
					// Clipboard integration is best-effort.
				}
			}

			if (outputSwatch)
			{
				var resolvedName = ResolveSwatchName(name, config.DefaultOutputNamePrefix);
				try
				{
					SwatchGenerator.Generate(resolvedName, palette, resolvedSquareSize);
				}
				catch
				{
					Console.WriteLine(Errors.SwatchFailure);
					context.ExitCode = ExitFailure;
				}
			}
		});

		return await rootCommand.InvokeAsync(args);
	}

	private static HsvColor CreateRandomMain()
	{
		var h = Random.Shared.NextDouble() * 360;
		var v = 50 + (Random.Shared.NextDouble() * 50);
		return new HsvColor(h, 70, v);
	}

	private static string ResolveSwatchName(string? name, string prefix)
	{
		var resolved = string.IsNullOrWhiteSpace(name) ? prefix : name.Trim();
		if (!resolved.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
			!resolved.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
		{
			resolved += ".jpg";
		}

		return resolved;
	}
}
