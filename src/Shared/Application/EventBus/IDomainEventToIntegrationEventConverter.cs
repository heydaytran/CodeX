using Domain.Primitives;

namespace Application.EventBus;

public interface IDomainEventToIntegrationEventConverter<TDomainEvent, TIntegrationEvent> 
    where TDomainEvent : IDomainEvent
    where TIntegrationEvent : IIntegrationEvent
{
    TIntegrationEvent Convert(TDomainEvent domainEvent);
}
