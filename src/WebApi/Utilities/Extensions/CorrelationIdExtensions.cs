namespace WebApi.Utilities.Extensions;

internal static class CorrelationIdExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}

internal class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private const string CorrelationIdHeader = "x-correlation-id";

    private static readonly ConcurrentDictionary<string, DateTime> _requestCache = new();
    private static readonly TimeSpan ExpiryTime = TimeSpan.FromMinutes(5); // Adjust expiration time

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        string correlationId;

        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out StringValues existingCorrelationId))
        {
            correlationId = existingCorrelationId.FirstOrDefault() ?? Guid.NewGuid().ToString();
        }
        else
        {
            correlationId = Guid.NewGuid().ToString();
        }

        // Check for duplicate Correlation ID
        if (_requestCache.ContainsKey(correlationId))
        {
            _logger.LogWarning("Duplicate request detected with Correlation ID: {CorrelationId}", correlationId);
            context.Response.StatusCode = StatusCodes.Status409Conflict; // 409 Conflict
            await context.Response.WriteAsync("Duplicate request detected.");
            return;
        }

        // Store Correlation ID with a timestamp
        _requestCache[correlationId] = DateTime.UtcNow;

        // Clean up expired requests
        CleanupExpiredRequests();

        context.Items[CorrelationIdHeader] = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        _logger.LogInformation("Request Correlation ID: {CorrelationId}", correlationId);

        await _next(context);
    }

    private void CleanupExpiredRequests()
    {
        var now = DateTime.UtcNow;
        foreach (var key in _requestCache.Keys)
        {
            if (_requestCache.TryGetValue(key, out var timestamp) && (now - timestamp) > ExpiryTime)
            {
                _requestCache.TryRemove(key, out _);
            }
        }
    }
}
