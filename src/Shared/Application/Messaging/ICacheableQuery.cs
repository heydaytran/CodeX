namespace Application.Messaging;

public interface ICacheableQuery
{
    bool BypassCache => false; 

    string CacheKey { get; } 

    bool DontCacheErrors => false; 

    TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(15); 

    TimeSpan? AbsoluteExpiration => TimeSpan.FromMinutes(15);
}

public interface ICacheableQuery<TResponse> : IQuery<TResponse>, ICacheableQuery
{
}