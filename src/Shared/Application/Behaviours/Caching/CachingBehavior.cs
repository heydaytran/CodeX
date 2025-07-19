using Application.Messaging;

namespace Application.Behaviours.Caching;

public class CachingBehavior<TRequest, TResponse>(IDistributedCache cache, ILogger<TResponse> logger, IOptionsSnapshot<CachingOptions> options, ICacheKeyGenerator<TRequest> cacheKeyGenerator) :
    IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheableQuery
        where TResponse : IErrorOr
{
    private static readonly JsonSerializerOptions DeserialiseOptions = new()
    {
        Converters = { new ErrorOrTValueJsonConverter(), }
    };

    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ICacheKeyGenerator<TRequest> _cacheKeyGenerator = cacheKeyGenerator;
    private readonly CachingOptions _options = options.Value;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return await next();
        }

        if (request is not ICacheableQuery cacheableQuery)
        {
            return await next();
        }

        if (cacheableQuery.BypassCache)
        {
            return await next();
        }

        var cacheKeyResult = await _cacheKeyGenerator.GenerateAsync(request, cancellationToken);
        if (cacheKeyResult.IsError)
        {
            return (TResponse)(dynamic)cacheKeyResult.Errors;
        }

        var cacheKey = cacheKeyResult.Value;

        TResponse? response;

        async Task<TResponse> GetResponseAndAddToCache()
        {
            response = await next();
            if (response.IsError && cacheableQuery.DontCacheErrors)
            {
                return response;
            }

            DistributedCacheEntryOptions options = _options.ToDistributedOptions(cacheableQuery);

            // In the future we should look at MessagePack for serialisation as a more performant option.
            var serialisedData = Encoding.Default.GetBytes(JsonSerializer.Serialize(response, DeserialiseOptions));

            await _cache.SetAsync(cacheKey, serialisedData, options, cancellationToken);

            return response;
        }

        var cachedResponse = await _cache.GetAsync(cacheKey, cancellationToken);
        if (cachedResponse != null)
        {
            string jsonString = Encoding.Default.GetString(cachedResponse);

            response = JsonSerializer.Deserialize<TResponse>(jsonString, DeserialiseOptions)!;
            _logger.LogDebug(
                "Response retrieved {TRequest} from cache. CacheKey: {CacheKey}",
                typeof(TRequest).FullName,
                cacheKey
            );
        }
        else
        {
            response = await GetResponseAndAddToCache();

            _logger.LogDebug(
                "Caching response for {TRequest} with cache key: {CacheKey}",
                typeof(TRequest).FullName,
                cacheKey
            );
        }

        return response;
    }
}
