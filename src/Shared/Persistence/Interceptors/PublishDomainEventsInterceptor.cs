using Domain.Primitives;
using Infrastructure.Extensions;

namespace Persistence.Interceptors;

public sealed class PublishDomainEventsInterceptor(IPublisher publisher) : SaveChangesInterceptor
{
    private readonly IPublisher _publisher = publisher ?? 
        throw new ArgumentNullException(nameof(publisher));

    private IEnumerable<IDomainEvent> _domainEvents = [];

    /// <inheritdoc />
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        var committedEvents = _domainEvents.Select(ICommittedDomainEvent.Create);
        await committedEvents.ForEachAsync(async d => await _publisher.Publish(d, cancellationToken));

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    /// <inheritdoc />
    public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        _domainEvents = GetDomainEvents(eventData.Context);
        await _domainEvents.ForEachAsync(async d => await _publisher.Publish(d));

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static List<IDomainEvent> GetDomainEvents(DbContext dbContext) =>
        dbContext
            .ChangeTracker
            .Entries<IEntity>()
            .Select(entityEntry => entityEntry.Entity)
            .SelectMany(entity => entity.GetDomainEvents().Tap(entity.ClearDomainEvents))
            .ToList();
}