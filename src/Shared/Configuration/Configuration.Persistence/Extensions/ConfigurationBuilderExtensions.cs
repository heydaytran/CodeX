using Configuration.Persistence.Provider;

namespace Configuration.Persistence.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddEfConfiguration<TDbContext>(
        this IConfigurationBuilder builder,
        Action<DbContextOptionsBuilder> optionsAction,
        bool reloadOnChange = false,
        TimeSpan? pollingInterval = null,
        string environment = "",
        string application = "",
        string target = "",
        bool addFirst = false,
        IConfigurationValueFormatter? valueFormatter = null,
        Action<EfConfigurationLoadExceptionContext<TDbContext>>? onLoadException = null) where TDbContext : DbContext
    {
        var source = new EfConfigurationSource<TDbContext>(optionsAction, reloadOnChange, pollingInterval, environment, application, target, valueFormatter, onLoadException);

        if (addFirst)
        {
            builder.Sources.Insert(0, source);
        }
        else
        {
            builder.Sources.Add(source);
        }

        return builder;
    }
}