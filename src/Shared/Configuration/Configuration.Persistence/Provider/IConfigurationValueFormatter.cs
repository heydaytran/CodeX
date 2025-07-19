namespace Configuration.Persistence.Provider;

public interface IConfigurationValueFormatter
{
    IDictionary<string, string?> FormatValues(IDictionary<string, string?> data);
}