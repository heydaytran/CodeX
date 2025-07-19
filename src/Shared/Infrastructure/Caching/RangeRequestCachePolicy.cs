namespace Infrastructure.Caching;

public sealed class RangeRequestCachePolicy(ILogger<RangeRequestCachePolicy> logger) : IOutputCachePolicy
{
    private readonly ILogger<RangeRequestCachePolicy> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public ValueTask CacheRequestAsync(
        OutputCacheContext context,
        CancellationToken cancellationToken)
    {
        var attemptOutputCaching = AttemptOutputCaching(context);
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = attemptOutputCaching;
        context.AllowCacheStorage = attemptOutputCaching;
        context.AllowLocking = true;

        // Vary by the range header.
        context.CacheVaryByRules.HeaderNames = HeaderNames.Range;

        _logger.LogDebug("Caching request for range request. Allowed to cache {AllowedToCache}.", attemptOutputCaching);

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation) => 
        ValueTask.CompletedTask;

    /// <inheritdoc/>
    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        var response = context.HttpContext.Response;

        context.AllowCacheStorage = 
            response.StatusCode == StatusCodes.Status200OK ||
            response.StatusCode == StatusCodes.Status206PartialContent;

        _logger.LogDebug("Serve response. Allowed to cache {AllowedToCache}.", context.AllowCacheStorage);

        return ValueTask.CompletedTask;
    }

    private static bool AttemptOutputCaching(OutputCacheContext context) => 
        HttpMethods.IsGet(context.HttpContext.Request.Method) ||
        HttpMethods.IsHead(context.HttpContext.Request.Method);
}