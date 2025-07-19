using Domain.Primitives;

namespace Application.Messaging;

public interface ICommittedDomainEventHandler<TEvent> : INotificationHandler<ICommittedDomainEvent<TEvent>>
    where TEvent : IDomainEvent
{
}