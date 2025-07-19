using Domain.Extensions;
using Domain.Primitives;
using Persistence.Extensions;

namespace Persistence.Interceptors;

public sealed class SetUserInterceptor(IPrincipalResolver principalResolver) : SaveChangesInterceptor
{
    private readonly IPrincipalResolver _principalResolver = principalResolver
        ?? throw new ArgumentNullException(nameof(principalResolver));

    /// <inheritdoc />
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await SetCurrentUserIdOnEntitiesAsync(eventData, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task SetCurrentUserIdOnEntitiesAsync(DbContextEventData eventData, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return;
        }

        var hasOptionalUserIdEntities = GetHasOptionalUserIdEntities(eventData.Context);
        var hasUserIdEntities = GetHasUserIdEntities(eventData.Context);

        if (!hasOptionalUserIdEntities.Any() && !hasUserIdEntities.Any())
        {
            return;
        }

        var result = await _principalResolver.ResolveAsync(cancellationToken);
        var userId = result.Value?.UserId();

        if (userId is null && hasUserIdEntities.Any())
        {
            result.ThrowIfError();

            throw new ArgumentException($"Unable to get user id from the current principal.");
        }

        if (userId != null)
        {
            foreach (var hasOptionalUserIdEntity in hasOptionalUserIdEntities)
            {
                hasOptionalUserIdEntity.Property(nameof(IHasOptionalUserId.UserId)).CurrentValue = userId;
            }
        }

        foreach (var hasUserIdEntity in hasUserIdEntities)
        {
            hasUserIdEntity.Property(nameof(IHasUserId.UserId)).CurrentValue = userId;
        }
    }

    private static IEnumerable<EntityEntry<IHasOptionalUserId>> GetHasOptionalUserIdEntities(DbContext dbContext)
        => dbContext.ChangeTracker.Entries<IHasOptionalUserId>();

    private static IEnumerable<EntityEntry<IHasUserId>> GetHasUserIdEntities(DbContext dbContext)
        => dbContext.ChangeTracker.Entries<IHasUserId>();
}