using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Hsvxp.Core;

public sealed class Config
{
    public int DefaultSquareSize { get; init; } = 512;
    public int DefaultMultiplier { get; init; } = 1;
    public string DefaultOutputNamePrefix { get; init; } = "hsvxp";

    public static Config Load(string? path, out bool invalid)
    {
        invalid = false;
        var filePath = string.IsNullOrWhiteSpace(path) ? "hsvxp.config.json" : path;

        if (!File.Exists(filePath))
        {
            return new Config();
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var configData = JsonSerializer.Deserialize<ConfigData>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            });

            if (configData is null)
            {
                invalid = true;
                return new Config();
            }

            return new Config
            {
                DefaultSquareSize = configData.DefaultSquareSize ?? 512,
                DefaultMultiplier = configData.DefaultMultiplier ?? 1,
                DefaultOutputNamePrefix = string.IsNullOrWhiteSpace(configData.DefaultOutputNamePrefix)
                    ? "hsvxp"
                    : configData.DefaultOutputNamePrefix.Trim()
            };
        }
        catch
        {
            invalid = true;
            return new Config();
        }
    }

    private sealed class ConfigData
    {
        [System.Text.Json.Serialization.JsonPropertyName("default_square_size")]
        public int? DefaultSquareSize { get; init; }

        [System.Text.Json.Serialization.JsonPropertyName("default_multiplier")]
        public int? DefaultMultiplier { get; init; }

        [System.Text.Json.Serialization.JsonPropertyName("default_output_name_prefix")]
        public string? DefaultOutputNamePrefix { get; init; }
    }
}
