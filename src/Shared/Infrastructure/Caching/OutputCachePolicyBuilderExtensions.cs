namespace Infrastructure.Caching;

public static class OutputCachePolicyBuilderExtensions
{
    public static OutputCachePolicyBuilder CacheRangeRequests(this OutputCachePolicyBuilder builder) => 
        builder.AddPolicy<RangeRequestCachePolicy>();
}