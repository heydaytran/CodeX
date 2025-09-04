using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Customer.Domain.Events;
using Modules.Customer.Persistence;
using Modules.Customer.Persistence.ReadModels;

namespace Modules.Customer.Application.Projections;

public sealed class CustomerProfileProjection :
    INotificationHandler<ICommittedDomainEvent<CustomerCreatedDomainEvent>>,
    INotificationHandler<ICommittedDomainEvent<CustomerDetailsUpdatedDomainEvent>>,
    INotificationHandler<ICommittedDomainEvent<CustomerPreferencesUpdatedDomainEvent>>,
    INotificationHandler<ICommittedDomainEvent<CustomerDeletedDomainEvent>>
{
    private readonly CustomerReadDbContext _dbContext;

    public CustomerProfileProjection(CustomerReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(ICommittedDomainEvent<CustomerCreatedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var e = notification.Event;

        _dbContext.CustomerProfiles.Add(new CustomerProfileReadModel
        {
            Id = e.CustomerId,
            Title = e.Title,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.ContactInformation.Email,
            EmailNotificationsEnabled = e.EmailNotificationsEnabled,
            SmsNotificationsEnabled = e.SmsNotificationsEnabled
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(ICommittedDomainEvent<CustomerDetailsUpdatedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var e = notification.Event;

        var profile = await _dbContext.CustomerProfiles.FirstOrDefaultAsync(p => p.Id == e.CustomerId, cancellationToken);
        if (profile is not null)
        {
            profile.Title = e.Title;
            profile.FirstName = e.FirstName;
            profile.LastName = e.LastName;
            profile.Email = e.ContactInformation.Email;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(ICommittedDomainEvent<CustomerPreferencesUpdatedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var e = notification.Event;

        var profile = await _dbContext.CustomerProfiles.FirstOrDefaultAsync(p => p.Id == e.CustomerId, cancellationToken);
        if (profile is not null)
        {
            profile.EmailNotificationsEnabled = e.EmailNotificationsEnabled;
            profile.SmsNotificationsEnabled = e.SmsNotificationsEnabled;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(ICommittedDomainEvent<CustomerDeletedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var e = notification.Event;

        var profile = await _dbContext.CustomerProfiles.FirstOrDefaultAsync(p => p.Id == e.CustomerId, cancellationToken);
        if (profile is not null)
        {
            _dbContext.CustomerProfiles.Remove(profile);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

