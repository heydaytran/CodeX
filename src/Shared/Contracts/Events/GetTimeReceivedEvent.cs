using Application.EventBus;

namespace Contracts.Events;

public record GetTimeReceivedEvent(DateTimeOffset OccurredOnUtc) : IntegrationEvent(OccurredOnUtc);