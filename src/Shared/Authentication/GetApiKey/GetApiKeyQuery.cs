namespace Authentication.GetApiKey;

public class GetApiKeyQuery(string apiKey) : ICacheableQuery<ApiKey>
{
    public string ApiKey { get; } = apiKey;
    public string CacheKey => BuildCacheKey.With(typeof(GetApiKeyQuery), ApiKey);
}