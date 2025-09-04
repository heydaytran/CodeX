using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Customer.Domain.Events;
using Modules.Customer.Persistence;
using Modules.Customer.Persistence.ReadModels;

namespace Modules.Customer.Application.Projections;

public sealed class CustomerNotificationPreferencesProjection :
    INotificationHandler<ICommittedDomainEvent<CustomerCreatedDomainEvent>>,
    INotificationHandler<ICommittedDomainEvent<CustomerPreferencesUpdatedDomainEvent>>,
    INotificationHandler<ICommittedDomainEvent<CustomerDeletedDomainEvent>>
{
    private readonly CustomerReadDbContext _dbContext;

    public CustomerNotificationPreferencesProjection(CustomerReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(ICommittedDomainEvent<CustomerCreatedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var e = notification.Event;

        _dbContext.CustomerNotificationPreferences.Add(new CustomerNotificationPreferencesReadModel
        {
            CustomerId = e.CustomerId,
            EmailNotificationsEnabled = e.EmailNotificationsEnabled,
            SmsNotificationsEnabled = e.SmsNotificationsEnabled
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(ICommittedDomainEvent<CustomerPreferencesUpdatedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var e = notification.Event;

        var prefs = await _dbContext.CustomerNotificationPreferences.FirstOrDefaultAsync(p => p.CustomerId == e.CustomerId, cancellationToken);
        if (prefs is not null)
        {
            prefs.EmailNotificationsEnabled = e.EmailNotificationsEnabled;
            prefs.SmsNotificationsEnabled = e.SmsNotificationsEnabled;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(ICommittedDomainEvent<CustomerDeletedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var e = notification.Event;

        var prefs = await _dbContext.CustomerNotificationPreferences.FirstOrDefaultAsync(p => p.CustomerId == e.CustomerId, cancellationToken);
        if (prefs is not null)
        {
            _dbContext.CustomerNotificationPreferences.Remove(prefs);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

