using System.Text;
using System.Text.Json;
using Application.Lifetimes;
using Domain.Primitives;
using MediatR;
using Modules.Customer.Domain.Abstractions;
using Modules.Customer.Domain.Entities;

namespace Modules.Customer.Infrastructure.EventStore;

public sealed class CustomerEventStore(IKurrentClient client, IMediator mediator) : ICustomerEventStore, IScoped
{
    private readonly IKurrentClient _client = client ?? throw new ArgumentNullException(nameof(client));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public async Task AppendAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        var events = customer.DomainEvents;
        var stream = $"customer-{customer.Id}";
        var payload = events.Select(e => new KurrentEvent(
            Guid.NewGuid(),
            e.GetType().AssemblyQualifiedName!,
            JsonSerializer.SerializeToUtf8Bytes(e, e.GetType(), _jsonOptions)));

        await _client.AppendAsync(stream, payload, customer.Version - events.Count, cancellationToken);

        foreach (var domainEvent in events)
        {
            var committedType = typeof(CommittedDomainEvent<>).MakeGenericType(domainEvent.GetType());
            var committed = Activator.CreateInstance(committedType, domainEvent);
            if (committed is INotification notification)
            {
                await _mediator.Publish(notification, cancellationToken);
            }
        }

        customer.ClearDomainEvents();
    }

    public async Task<IReadOnlyList<IDomainEvent>> LoadAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var stream = $"customer-{customerId}";
        var messages = await _client.ReadAsync(stream, cancellationToken);
        var events = new List<IDomainEvent>();
        foreach (var message in messages)
        {
            var type = Type.GetType(message.Type);
            if (type is null)
            {
                continue;
            }

            var domainEvent = (IDomainEvent?)JsonSerializer.Deserialize(message.Data.Span, type, _jsonOptions);
            if (domainEvent is not null)
            {
                events.Add(domainEvent);
            }
        }

        return events;
    }

    public async Task<IReadOnlyList<StoredEvent>> GetHistoryAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var stream = $"customer-{customerId}";
        var messages = await _client.ReadAsync(stream, cancellationToken);
        var history = new List<StoredEvent>();
        foreach (var message in messages)
        {
            var data = Encoding.UTF8.GetString(message.Data.Span);
            history.Add(new StoredEvent(message.Type, data, message.Version, message.OccurredOnUtc));
        }
        return history;
    }
}

public record KurrentEvent(Guid Id, string Type, byte[] Data);

public record KurrentMessage(string Type, ReadOnlyMemory<byte> Data, long Version, DateTimeOffset OccurredOnUtc);

public interface IKurrentClient
{
    Task AppendAsync(string stream, IEnumerable<KurrentEvent> events, long expectedVersion, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KurrentMessage>> ReadAsync(string stream, CancellationToken cancellationToken = default);
}
