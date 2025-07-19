using Application.Messaging;

namespace Application.Behaviours.Caching;

public class DefaultCacheKeyGenerator<TQuery> : ICacheKeyGenerator<TQuery>
    where TQuery : ICacheableQuery
{
    /// <inheritdoc />
    public Task<ErrorOr<string>> GenerateAsync(TQuery query, CancellationToken cancellationToken) => 
        Task.FromResult<ErrorOr<string>>(query.CacheKey);
}