using Application.Lifetimes;
using Domain.Primitives;

namespace Persistence.Interceptors;

public sealed class UpdateAuditableEntitiesInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor, ISingleton
{
    private readonly TimeProvider _timeProvider = timeProvider 
        ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <inheritdoc/>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData);

        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContextEventData eventData)
    {
        if (eventData.Context is null)
        {
            return;
        }

        DateTime utcNow = _timeProvider.GetUtcNow().DateTime;

        foreach (EntityEntry<IBaseAuditable> auditable in GetAuditableEntities(eventData.Context))
        {
            if (auditable.State == EntityState.Added && auditable.Entity is IAuditableCreatedAt)
            {
                auditable.Property(nameof(IAuditableCreatedAt.CreatedAt)).CurrentValue = utcNow;
            }

            if (auditable.Entity is IAuditableLastActivityAt)
            {
                auditable.Property(nameof(IAuditableLastActivityAt.LastActivityAt)).CurrentValue = utcNow;
            }
        }
    }

    private static IEnumerable<EntityEntry<IBaseAuditable>> GetAuditableEntities(DbContext dbContext) 
        => dbContext.ChangeTracker.Entries<IBaseAuditable>();
}