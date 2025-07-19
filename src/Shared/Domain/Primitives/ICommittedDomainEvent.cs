namespace Domain.Primitives;

public interface ICommittedDomainEvent : INotification
{
    public static ICommittedDomainEvent Create(IDomainEvent @event)
    {
        var type = @event.GetType();
        var committedEventType = typeof(CommittedDomainEvent<>).MakeGenericType(type);
        var committedEvent = Activator.CreateInstance(committedEventType, @event) as ICommittedDomainEvent;
        return committedEvent ?? throw new InvalidOperationException($"Unable to create committed domain event for type {type}.");
    }
}

public interface ICommittedDomainEvent<TDomainEvent> : ICommittedDomainEvent where TDomainEvent : IDomainEvent
{
    TDomainEvent Event { get; }
}