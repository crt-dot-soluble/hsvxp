using Hsvxp.Core;

namespace Hsvxp;

public static class WindowsCompletionRegistrar
{
    public static void TryRegister(string commandName, IEnumerable<string> optionAliases)
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        try
        {
            var script = CompletionScriptBuilder.BuildPowerShell(commandName, optionAliases);
            foreach (var profilePath in GetProfilePaths())
            {
                RegisterProfile(profilePath, script);
            }
        }
        catch
        {
            // Autocomplete registration is best-effort.
        }
    }

    private static IEnumerable<string> GetProfilePaths()
    {
        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (string.IsNullOrWhiteSpace(documents))
        {
            yield break;
        }

        yield return Path.Combine(documents, "WindowsPowerShell", "Microsoft.PowerShell_profile.ps1");
        yield return Path.Combine(documents, "PowerShell", "Microsoft.PowerShell_profile.ps1");
    }

    private static void RegisterProfile(string profilePath, string script)
    {
        var directory = Path.GetDirectoryName(profilePath);
        if (string.IsNullOrWhiteSpace(directory))
        {
            return;
        }

        Directory.CreateDirectory(directory);

        if (File.Exists(profilePath))
        {
            var existing = File.ReadAllText(profilePath);
            if (existing.Contains(CompletionScriptBuilder.Marker, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }

        File.AppendAllText(profilePath, script);
    }
}
