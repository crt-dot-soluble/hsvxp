using System.CommandLine;
using System.CommandLine.Parsing;
using Hsvxp;
using Hsvxp.Core;
using TextCopy;

internal static class Program
{
	private const int ExitFailure = 1;

	public static async Task<int> Main(string[] args)
	{
		var colorArgument = new Argument<string?>("color")
		{
			Arity = ArgumentArity.ZeroOrOne,
			Description = "Input color in rgb(), hsv(), hex(), #RGB, or #RRGGBB format."
		};

		var outputSwatchOption = new Option<bool>("--output-swatch", new[] { "-o" })
		{
			Description = "Write JPEG swatch output."
		};

		var squareSizeOption = new Option<int?>("--square-size", new[] { "-s" })
		{
			Description = "Square size in pixels."
		};

		var nameOption = new Option<string?>("--name", new[] { "-n" })
		{
			Description = "Swatch filename."
		};

		var swatchOrientationOption = new Option<string?>("--swatch-orientation", new[] { "-O" })
		{
			Description = "Swatch orientation: columns or rows."
		};

		var copyOption = new Option<bool>("--copy", new[] { "-c" })
		{
			Description = "Copy output to clipboard (Windows only)."
		};

		var randomOption = new Option<bool>("--random", new[] { "-r" })
		{
			Description = "Generate a random base color."
		};

		var invertOption = new Option<bool>("--invert", new[] { "-i" })
		{
			Description = "Invert the input RGB color before HSV normalization."
		};

		var grayscaleOption = new Option<bool>("--grayscale", new[] { "-g" })
		{
			Description = "Force grayscale palette generation."
		};

		var grayscaleImageOption = new Option<string?>("--grayscale-image", new[] { "-G" })
		{
			Description = "Grayscale an input image and write a new file."
		};

		var jsonOption = new Option<bool>("--json", new[] { "-j" })
		{
			Description = "Output JSON."
		};

		var rawOption = new Option<bool>("--raw", new[] { "-R" })
		{
			Description = "Output raw HSV."
		};

		var configOption = new Option<string?>("--config", new[] { "-C" })
		{
			Description = "Path to hsvxp.config.json."
		};

		var noWindowsCompletionsOption = new Option<bool>("--no-windows-completions", new[] { "-W" })
		{
			Description = "Skip Windows autocomplete registration."
		};

		var multiplierOption = new Option<int?>("--multiplier", new[] { "-m" })
		{
			Description = "Palette multiplier 1-16."
		};

		var rootCommand = new RootCommand("HSVXP palette generator")
		{
			colorArgument,
			outputSwatchOption,
			squareSizeOption,
			nameOption,
			swatchOrientationOption,
			copyOption,
			randomOption,
			invertOption,
			grayscaleOption,
			grayscaleImageOption,
			jsonOption,
			rawOption,
			configOption,
			noWindowsCompletionsOption,
			multiplierOption
		};

		var parseResult = rootCommand.Parse(args);
		if (parseResult.Errors.Count > 0)
		{
			Console.WriteLine(Errors.InvalidColor);
			return ExitFailure;
		}

		var parsedColor = parseResult.GetValue(colorArgument);
		var outputSwatch = parseResult.GetValue(outputSwatchOption);
		var squareSize = parseResult.GetValue(squareSizeOption);
		var name = parseResult.GetValue(nameOption);
		var swatchOrientation = parseResult.GetValue(swatchOrientationOption);
		var copy = parseResult.GetValue(copyOption);
		var random = parseResult.GetValue(randomOption);
		var invert = parseResult.GetValue(invertOption);
		var grayscale = parseResult.GetValue(grayscaleOption);
		var grayscaleImage = parseResult.GetValue(grayscaleImageOption);
		var json = parseResult.GetValue(jsonOption);
		var raw = parseResult.GetValue(rawOption);
		var configPath = parseResult.GetValue(configOption);
		var noWindowsCompletions = parseResult.GetValue(noWindowsCompletionsOption);
		var multiplier = parseResult.GetValue(multiplierOption);

		var optionAliases = new[]
		{
			"-o", "--output-swatch",
			"-s", "--square-size",
			"-n", "--name",
			"-O", "--swatch-orientation",
			"-c", "--copy",
			"-r", "--random",
			"-i", "--invert",
			"-g", "--grayscale",
			"-G", "--grayscale-image",
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
			return ExitFailure;
		}

		var resolvedSquareSize = squareSize ?? config.DefaultSquareSize;
		if (!TryParseOrientation(swatchOrientation, out var resolvedOrientation))
		{
			Console.WriteLine(Errors.InvalidSwatchOrientation);
			return ExitFailure;
		}

		var imageOnly = !random && string.IsNullOrWhiteSpace(parsedColor) && !string.IsNullOrWhiteSpace(grayscaleImage);
		if (imageOnly)
		{
			if (!ImageGrayscaleConverter.TryConvert(grayscaleImage, out _))
			{
				Console.WriteLine(Errors.InvalidGrayscaleImage);
				return ExitFailure;
			}

			return 0;
		}

		if (!random && string.IsNullOrWhiteSpace(parsedColor))
		{
			Console.WriteLine(Errors.MissingColor);
			return ExitFailure;
		}

		InputColor? inputColor = null;
		if (!random)
		{
			if (!ColorParser.TryParse(parsedColor!, out var parsed))
			{
				Console.WriteLine(Errors.InvalidColor);
				return ExitFailure;
			}

			inputColor = parsed;
		}

		var main = random
			? CreateRandomMain()
			: inputColor!.Hsv;

		if (invert)
		{
			var rgb = random
				? ColorMath.HsvToRgb(main)
				: inputColor!.Rgb;

			var inverted = new RgbColor((byte)(255 - rgb.R), (byte)(255 - rgb.G), (byte)(255 - rgb.B));
			var invertedHsv = ColorMath.RgbToHsv(inverted);
			main = invertedHsv;
		}

		var palette = PaletteGenerator.Generate(main, resolvedMultiplier, grayscale);
		var output = json
			? OutputFormatter.FormatJson(palette)
			: raw
				? OutputFormatter.FormatRaw(palette)
					: OutputFormatter.FormatText(palette);

		if (json || raw)
		{
			Console.WriteLine(output);
		}
		else
		{
			PaletteConsoleWriter.WriteText(palette);
		}

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
				SwatchGenerator.Generate(resolvedName, palette, resolvedSquareSize, resolvedOrientation);
			}
			catch
			{
				Console.WriteLine(Errors.SwatchFailure);
				return ExitFailure;
			}
		}

		if (!string.IsNullOrWhiteSpace(grayscaleImage))
		{
			if (!ImageGrayscaleConverter.TryConvert(grayscaleImage, out _))
			{
				Console.WriteLine(Errors.InvalidGrayscaleImage);
				return ExitFailure;
			}
		}

		return 0;
	}

	private static bool TryParseOrientation(string? value, out SwatchOrientation orientation)
	{
		orientation = SwatchOrientation.Columns;
		if (string.IsNullOrWhiteSpace(value))
		{
			return true;
		}

		return value.Trim().ToLowerInvariant() switch
		{
			"columns" => (orientation = SwatchOrientation.Columns) == SwatchOrientation.Columns,
			"rows" => (orientation = SwatchOrientation.Rows) == SwatchOrientation.Rows,
			_ => false
		};
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
