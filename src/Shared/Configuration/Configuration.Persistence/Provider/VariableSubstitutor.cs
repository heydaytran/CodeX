using System.Text.RegularExpressions;

namespace Configuration.Persistence.Provider;

public static class VariableSubstitutor
{
    public static string Substitute(IDictionary<string, string?> data, string value, HashSet<string>? substitutedVariables = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (!value.Contains('$', StringComparison.OrdinalIgnoreCase))
        {
            return value;
        }

        substitutedVariables ??= [];

        string ReplaceVariable(Match match)
        {
            string variableName = match.Groups[1].Value;
            if (substitutedVariables.Contains(variableName))
            {
                // Skip if the variable has already been substituted to prevent infinite loops.
                return match.Value;
            }

            if (data.TryGetValue(variableName, out var replacement))
            {
                // Mark the variable as substituted.
                substitutedVariables.Add(variableName);

                // Recursively substitute variables in the replacement value.
                return Substitute(data, replacement ?? string.Empty, substitutedVariables);
            }

            // If the variable is not found, return the original match.
            return match.Value;
        }

        string pattern = @"\$([^$]+)\$";

        // Perform substitution iteratively until no more replacements are made.
        bool replaced;
        do
        {
            string previousValue = value;
            value = Regex.Replace(value, pattern, ReplaceVariable);
            replaced = value != previousValue;
        } while (replaced);

        return value;
    }
}
