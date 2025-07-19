using Application.EventBus;

namespace Contracts.Events.Checkouts;

public record BasketCreatedEvent(DateTimeOffset OccurredOnUtc, Guid BasketId) : IntegrationEvent(OccurredOnUtc);