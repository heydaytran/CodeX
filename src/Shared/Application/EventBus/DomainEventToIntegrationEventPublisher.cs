using Application.Messaging;
using Domain.Primitives;

namespace Application.EventBus;

public sealed class DomainEventToIntegrationEventPublisher<TDomainEvent, TIntegrationEvent>(IEventBus eventBus, IDomainEventToIntegrationEventConverter<TDomainEvent, TIntegrationEvent> converter) : 
    ICommittedDomainEventHandler<TDomainEvent>
        where TDomainEvent : IDomainEvent
        where TIntegrationEvent : IIntegrationEvent
{
    private readonly IEventBus _eventBus = eventBus
        ?? throw new ArgumentNullException(nameof(eventBus));

    private readonly IDomainEventToIntegrationEventConverter<TDomainEvent, TIntegrationEvent> _converter = converter
        ?? throw new ArgumentNullException(nameof(converter));

    /// <inheritdoc/>
    public Task Handle(ICommittedDomainEvent<TDomainEvent> notification, CancellationToken cancellationToken) => 
        _eventBus.PublishAsync(_converter.Convert(notification.Event), cancellationToken);
}