namespace Application.EventBus;

public interface IEventBus
{
    /// <summary>
    /// Publishes the specified integration event to the event bus.
    /// </summary>
    /// <typeparam name="TIntegrationEvent">The integration event type.</typeparam>
    /// <param name="integrationEvent">The integration event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the publish operation.</returns>
    Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        where TIntegrationEvent : IIntegrationEvent;
}