namespace Configuration.Persistence.Provider;

internal sealed class EfConfigurationSource<TDbContext> : IConfigurationSource
    where TDbContext : DbContext
{
    public readonly Action<DbContextOptionsBuilder> OptionsAction;
    public readonly bool ReloadOnChange;
    public readonly TimeSpan PollingInterval;
    public readonly string Environment;
    public readonly string Application;
    public readonly string Target;
    public readonly IConfigurationValueFormatter? ValueFormatter;
    public readonly Action<EfConfigurationLoadExceptionContext<TDbContext>>? OnLoadException;

    public EfConfigurationSource(
        Action<DbContextOptionsBuilder> optionsAction,
        bool reloadOnChange = false,
        TimeSpan? pollingInterval = null,
        string environment = "",
        string application = "",
        string target = "",
        IConfigurationValueFormatter? valueFormatter = null,
        Action<EfConfigurationLoadExceptionContext<TDbContext>>? onLoadException = null)
    {
        PollingInterval = pollingInterval ?? TimeSpan.FromMinutes(1);

        if (PollingInterval.TotalMilliseconds < 500)
        {
            throw new ArgumentException($"{nameof(pollingInterval)} can not less than 500.", nameof(pollingInterval));
        }

        OptionsAction = optionsAction;
        ReloadOnChange = reloadOnChange;
        Environment = environment;
        Application = application;
        Target = target;
        ValueFormatter = valueFormatter;
        OnLoadException = onLoadException;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new EfConfigurationProvider<TDbContext>(this);
    }
}
