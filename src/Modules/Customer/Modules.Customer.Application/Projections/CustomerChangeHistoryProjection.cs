using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Customer.Domain.Events;
using Modules.Customer.Persistence;
using Modules.Customer.Persistence.ReadModels;

namespace Modules.Customer.Application.Projections;

public sealed class CustomerChangeHistoryProjection :
    INotificationHandler<ICommittedDomainEvent<CustomerCreatedDomainEvent>>,
    INotificationHandler<ICommittedDomainEvent<CustomerDetailsUpdatedDomainEvent>>,
    INotificationHandler<ICommittedDomainEvent<CustomerPreferencesUpdatedDomainEvent>>,
    INotificationHandler<ICommittedDomainEvent<CustomerDeletedDomainEvent>>
{
    private readonly CustomerReadDbContext _dbContext;

    public CustomerChangeHistoryProjection(CustomerReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(ICommittedDomainEvent<CustomerCreatedDomainEvent> notification, CancellationToken cancellationToken)
    {
        await AddChange(notification.Event.CustomerId, nameof(CustomerCreatedDomainEvent), notification.Event.OccurredOnUtc, cancellationToken);
    }

    public async Task Handle(ICommittedDomainEvent<CustomerDetailsUpdatedDomainEvent> notification, CancellationToken cancellationToken)
    {
        await AddChange(notification.Event.CustomerId, nameof(CustomerDetailsUpdatedDomainEvent), notification.Event.OccurredOnUtc, cancellationToken);
    }

    public async Task Handle(ICommittedDomainEvent<CustomerPreferencesUpdatedDomainEvent> notification, CancellationToken cancellationToken)
    {
        await AddChange(notification.Event.CustomerId, nameof(CustomerPreferencesUpdatedDomainEvent), notification.Event.OccurredOnUtc, cancellationToken);
    }

    public async Task Handle(ICommittedDomainEvent<CustomerDeletedDomainEvent> notification, CancellationToken cancellationToken)
    {
        await AddChange(notification.Event.CustomerId, nameof(CustomerDeletedDomainEvent), notification.Event.OccurredOnUtc, cancellationToken);
    }

    private async Task AddChange(Guid customerId, string type, DateTimeOffset occurredOnUtc, CancellationToken cancellationToken)
    {
        var version = await _dbContext.CustomerChanges
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.Version)
            .Select(c => c.Version)
            .FirstOrDefaultAsync(cancellationToken) + 1;

        _dbContext.CustomerChanges.Add(new CustomerChangeReadModel
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Version = version,
            Type = type,
            OccurredOnUtc = occurredOnUtc
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

