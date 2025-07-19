namespace WebApi.Utilities.Logging.Enrichers;

internal sealed class RequestHeadersEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly string[]? _exclude;

    public RequestHeadersEnricher(string[]? exclude)
        : this(new HttpContextAccessor())
    {
        _exclude = exclude;
    }

    internal RequestHeadersEnricher(IHttpContextAccessor contextAccessor) => 
        _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));

    /// <inheritdoc/>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _contextAccessor.HttpContext;
        if (httpContext == null)
        {
            return;
        }

        var headers = httpContext.Request.Headers.Where(h => !string.IsNullOrEmpty(h.Key));
        if (_exclude is not null)
        {
            headers = headers.Where(h => !_exclude.Contains(h.Key, StringComparer.OrdinalIgnoreCase));
        } 

        headers.ForEach(h => AddHeader(logEvent, httpContext, h.Key, h.Value.ToString()));
    }

    private static void AddHeader(LogEvent logEvent, HttpContext httpContext, string headerKey, string? headerValue)
    {
        string propertyName = headerKey.Replace("-", "");
        string clientHeaderItemKey = $"Serilog_{headerKey}";

        if (httpContext.Items[clientHeaderItemKey] is LogEventProperty logEventProperty)
        {
            logEvent.AddPropertyIfAbsent(logEventProperty);
            return;
        }

        headerValue = string.IsNullOrWhiteSpace(headerValue) ? null : headerValue;

        var logProperty = new LogEventProperty(propertyName, new ScalarValue(headerValue));
        httpContext.Items.Add(clientHeaderItemKey, logProperty);

        logEvent.AddPropertyIfAbsent(logProperty);
    }
}