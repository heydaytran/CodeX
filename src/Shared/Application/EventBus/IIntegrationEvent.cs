namespace Application.EventBus;

public interface IIntegrationEvent
{
    DateTimeOffset OccurredOnUtc { get; }
}