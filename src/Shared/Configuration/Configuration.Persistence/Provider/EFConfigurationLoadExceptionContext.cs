namespace Configuration.Persistence.Provider;

public sealed class EfConfigurationLoadExceptionContext<TDbContext>
    where TDbContext : DbContext
{
    public Exception Exception { get; }

    public bool Ignore { get; set; }

    internal EfConfigurationSource<TDbContext> Source { get; }

    internal EfConfigurationLoadExceptionContext(EfConfigurationSource<TDbContext> source, Exception exception)
    {
        Source = source;
        Exception = exception;
    }
}