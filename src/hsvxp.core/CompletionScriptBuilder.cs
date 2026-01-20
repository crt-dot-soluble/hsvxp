using System.Text;

namespace Hsvxp.Core;

public static class CompletionScriptBuilder
{
    public const string Marker = "# hsvxp completions";

    public static string BuildPowerShell(string commandName, IEnumerable<string> options)
    {
        var uniqueOptions = options.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(option => option).ToArray();
        var builder = new StringBuilder();

        builder.AppendLine();
        builder.AppendLine(Marker);
        builder.AppendLine("Register-ArgumentCompleter -Native -CommandName \"" + commandName + "\" -ScriptBlock {");
        builder.AppendLine("    param($wordToComplete, $commandAst, $cursorPosition)");
        builder.AppendLine("    $options = @(");

        for (var i = 0; i < uniqueOptions.Length; i++)
        {
            var option = uniqueOptions[i].Replace("\"", "\"\"");
            var suffix = i == uniqueOptions.Length - 1 ? string.Empty : ",";
            builder.AppendLine("        \"" + option + "\"" + suffix);
        }

        builder.AppendLine("    )");
        builder.AppendLine("    $options | Where-Object { $_ -like ($wordToComplete + '*') } | ForEach-Object {");
        builder.AppendLine("        [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterName', $_)");
        builder.AppendLine("    }");
        builder.AppendLine("}");

        return builder.ToString();
    }
}
