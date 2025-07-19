
namespace Configuration.Persistence.Provider;

public class ConfigurationValueFormatter : IConfigurationValueFormatter
{
    private const string EnvironmentVariable = "$environment_name$";
    private const string GlobalEnvironmentVariable = "$Global.Environment.Name$";
    private const string GlobalSuffixVariable = "$Global.Urls.DomainSuffix$";

    private const string EnvironmentConfigurationName = "Global.Environment.Name";
    private const string GlobalDomainSuffixKey = "Global.Urls.DomainSuffix";

    private static readonly IDictionary<string, string> ReplaceMap = new Dictionary<string, string>
    {
        { EnvironmentVariable, EnvironmentConfigurationName },
        { GlobalEnvironmentVariable, EnvironmentConfigurationName },
        { GlobalSuffixVariable, GlobalDomainSuffixKey },
    };

    public IDictionary<string, string?> FormatValues(IDictionary<string, string?> data)
    {
        if (data == null)
        {
            return data ?? new Dictionary<string, string?>();
        }

        return data.ToDictionary(kv => kv.Key, kv => DoReplace(data, kv.Value));
    }

    private static string? DoReplace(IDictionary<string, string?> data, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        foreach (KeyValuePair<string, string> replace in ReplaceMap)
        {
            value = ReplaceCaseInsensitive(value, replace.Key, x => data.TryGetValue(replace.Value, out string? value) ? value ?? string.Empty : string.Empty, true);
        }

        value = VariableSubstitutor.Substitute(data, value);

        return value;
    }

    private static string ReplaceCaseInsensitive(string target, string searchValue, Func<string, string> replacementValue, bool allowMultipleReplacements)
    {
        var cIndex = CultureInfo.CurrentCulture.CompareInfo.IndexOf(target, searchValue, CompareOptions.IgnoreCase);
        if (cIndex < 0)
        {
            return target;
        }

        var endIndex = cIndex + searchValue.Length;
        var endString = allowMultipleReplacements
            ? ReplaceCaseInsensitive(target[endIndex..], searchValue, replacementValue, true)
            : target[endIndex..];
        return string.Concat(target.AsSpan(0, cIndex), replacementValue(searchValue), endString);
    }
}
