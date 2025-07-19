using Application.EventBus;
using Application.Lifetimes;

namespace Infrastructure.EventBus;

public sealed class EventBus(IBus bus) : IEventBus, ITransient
{
    private readonly IBus _bus = bus ?? throw new ArgumentNullException(nameof(bus));

    /// <inheritdoc />
    public async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        where TIntegrationEvent : IIntegrationEvent =>
        await _bus.Publish(integrationEvent, cancellationToken);
}