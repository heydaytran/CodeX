using MediatR;

namespace Domain.Primitives;

public abstract record DomainEvent : IDomainEvent
{
    public Guid Id { get; }

    protected DomainEvent(DateTimeOffset occurredOnUtc)
    {
        Id = Guid.NewGuid();
        OccurredOnUtc = occurredOnUtc;
    }

    /// <inheritdoc />
    public DateTimeOffset OccurredOnUtc { get; private set; }
}

public record CommittedDomainEvent<TDomainEvent>(TDomainEvent Event) : ICommittedDomainEvent<TDomainEvent> 
    where TDomainEvent : IDomainEvent
{
}