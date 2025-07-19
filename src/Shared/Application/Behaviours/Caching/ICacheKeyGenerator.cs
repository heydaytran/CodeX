using Application.Messaging;

namespace Application.Behaviours.Caching;

public interface ICacheKeyGenerator<TQuery> where TQuery : ICacheableQuery
{
    Task<ErrorOr<string>> GenerateAsync(TQuery query, CancellationToken cancellationToken);
}