using Domain.Extensions;
using Persistence.Interceptors;

namespace Tenant.Persistence.Interceptors;

public sealed class CacheCleanerInterceptor(IDistributedCache cache, IEnumerable<ICacheCleanerKeyCollector> collectors) 
    : SaveChangesInterceptor
{
    private readonly IDistributedCache _cache = cache ?? 
        throw new ArgumentNullException(nameof(cache));

    private readonly IEnumerable<ICacheCleanerKeyCollector> _collectors = collectors ??
        throw new ArgumentNullException(nameof(collectors));

    /// <inheritdoc />
    public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var affected = await base.SavingChangesAsync(eventData, result, cancellationToken);

        await RemoveAffectedEntitiesKeys(eventData, cancellationToken);

        return affected;
    }

    private async Task RemoveAffectedEntitiesKeys(DbContextEventData eventData, CancellationToken cancellationToken = default)
    {
        var changeTracker = eventData.Context?.ChangeTracker;
        if (changeTracker is null)
        {
            return;
        }

        foreach (var collector in _collectors)
        {
            var keys = collector.KeysToClean(changeTracker);
            keys.ThrowIfError();

            foreach (var key in keys.Value)
            {
                await _cache.RemoveAsync(key, cancellationToken);
            }
        }
    }
}