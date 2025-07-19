namespace Application.EventBus;

public abstract record IntegrationEvent(DateTimeOffset OccurredOnUtc) : IIntegrationEvent;