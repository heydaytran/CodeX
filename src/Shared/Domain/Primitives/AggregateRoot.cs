namespace Domain.Primitives;

/// <summary>
/// Base class for aggregate roots supporting event sourcing.
/// </summary>
/// <typeparam name="TEntityId">The entity identifier type.</typeparam>
public abstract class AggregateRoot<TEntityId> : Entity<TEntityId>, IAggregate<TEntityId>
    where TEntityId : notnull
{
    protected AggregateRoot() { }

    protected AggregateRoot(TEntityId id) : base(id) { }

    /// <inheritdoc />
    public IReadOnlyList<IDomainEvent> DomainEvents => GetDomainEvents();

    /// <inheritdoc />
    public long Version { get; set; } = -1;

    /// <summary>
    /// Applies and records a new domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    protected void ApplyChange(IDomainEvent domainEvent)
    {
        When(domainEvent);
        RaiseDomainEvent(domainEvent);
        Version++;
    }

    /// <summary>
    /// Applies a historical event without recording it.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    protected void Apply(IDomainEvent domainEvent)
    {
        When(domainEvent);
        Version++;
    }

    /// <summary>
    /// Loads the aggregate state from a sequence of domain events.
    /// </summary>
    /// <param name="history">The domain events history.</param>
    public void Load(IEnumerable<IDomainEvent> history)
    {
        foreach (var domainEvent in history)
        {
            Apply(domainEvent);
        }
    }

    private void When(IDomainEvent domainEvent) => ((dynamic)this).On((dynamic)domainEvent);
}

