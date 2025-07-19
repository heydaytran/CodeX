using Application.EventBus;
using Infrastructure.Extensions;

namespace Infrastructure.EventBus;

public sealed class IntegrationEventConsumer<TIntegrationEvent>(IEnumerable<IIntegrationEventHandler<TIntegrationEvent>> handlers) : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : class, IIntegrationEvent
{
    private readonly IEnumerable<IIntegrationEventHandler<TIntegrationEvent>> _handlers = handlers
        ?? throw new ArgumentNullException(nameof(handlers));

    /// <inheritdoc />
    public async Task Consume(ConsumeContext<TIntegrationEvent> context) =>
        await _handlers.ForEachAsync(async h => await h.Handle(context.Message, context.CancellationToken));
}