using System.Text.Json;
using System.Linq;
using Application.Lifetimes;
using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Customer.Domain.Abstractions;
using Modules.Customer.Domain.Entities;
using Modules.Customer.Persistence;
using Modules.Customer.Persistence.Entities;

namespace Modules.Customer.Infrastructure.EventStore;

public sealed class CustomerEventStore(CustomerDbContext dbContext, IMediator mediator) : ICustomerEventStore, IScoped
{
    private readonly CustomerDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public async Task AppendAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        var events = customer.DomainEvents;
        var lastVersion = await _dbContext.CustomerEvents
            .Where(e => e.CustomerId == customer.Id)
            .OrderByDescending(e => e.Version)
            .Select(e => e.Version)
            .FirstOrDefaultAsync(cancellationToken);

        if (customer.Version - events.Count != lastVersion)
        {
            throw new InvalidOperationException("Concurrency conflict detected.");
        }

        var nextVersion = lastVersion;
        foreach (var domainEvent in events)
        {
            nextVersion++;
            var serialized = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), _jsonOptions);
            _dbContext.CustomerEvents.Add(new CustomerEvent
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                Type = domainEvent.GetType().AssemblyQualifiedName!,
                Data = serialized,
                Version = nextVersion,
                OccurredOnUtc = domainEvent.OccurredOnUtc
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

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
        var eventEntities = await _dbContext.CustomerEvents
            .Where(e => e.CustomerId == customerId)
            .OrderBy(e => e.Version)
            .ToListAsync(cancellationToken);

        var events = new List<IDomainEvent>();
        foreach (var entity in eventEntities)
        {
            var type = Type.GetType(entity.Type);
            if (type is null)
            {
                continue;
            }

            var domainEvent = (IDomainEvent?)JsonSerializer.Deserialize(entity.Data, type, _jsonOptions);
            if (domainEvent is not null)
            {
                events.Add(domainEvent);
            }
        }

        return events;
    }

    public async Task<IReadOnlyList<StoredEvent>> GetHistoryAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var eventEntities = await _dbContext.CustomerEvents
            .Where(e => e.CustomerId == customerId)
            .OrderBy(e => e.Version)
            .ToListAsync(cancellationToken);

        return eventEntities
            .Select(e => new StoredEvent(e.Type, e.Data, e.Version, e.OccurredOnUtc))
            .ToList();
    }
}

