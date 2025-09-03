using Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Customer.Domain.Events;
using Modules.Customer.Persistence;
using Modules.Customer.Persistence.ReadModels;

namespace Modules.Customer.Application.Projections;

public sealed class CustomerReadModelProjection :
    INotificationHandler<ICommittedDomainEvent<CustomerCreatedDomainEvent>>,
    INotificationHandler<ICommittedDomainEvent<CustomerDetailsUpdatedDomainEvent>>,
    INotificationHandler<ICommittedDomainEvent<CustomerPreferencesUpdatedDomainEvent>>,
    INotificationHandler<ICommittedDomainEvent<CustomerDeletedDomainEvent>>
{
    private readonly CustomerReadDbContext _dbContext;

    public CustomerReadModelProjection(CustomerReadDbContext dbContext)
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

        _dbContext.CustomerNotificationPreferences.Add(new CustomerNotificationPreferencesReadModel
        {
            CustomerId = e.CustomerId,
            EmailNotificationsEnabled = e.EmailNotificationsEnabled,
            SmsNotificationsEnabled = e.SmsNotificationsEnabled
        });

        _dbContext.CustomerChanges.Add(new CustomerChangeReadModel
        {
            Id = Guid.NewGuid(),
            CustomerId = e.CustomerId,
            Version = 1,
            Type = nameof(CustomerCreatedDomainEvent),
            OccurredOnUtc = e.OccurredOnUtc
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

        var version = await _dbContext.CustomerChanges
            .Where(c => c.CustomerId == e.CustomerId)
            .OrderByDescending(c => c.Version)
            .Select(c => c.Version)
            .FirstOrDefaultAsync(cancellationToken) + 1;

        _dbContext.CustomerChanges.Add(new CustomerChangeReadModel
        {
            Id = Guid.NewGuid(),
            CustomerId = e.CustomerId,
            Version = version,
            Type = nameof(CustomerDetailsUpdatedDomainEvent),
            OccurredOnUtc = e.OccurredOnUtc
        });

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

        var prefs = await _dbContext.CustomerNotificationPreferences.FirstOrDefaultAsync(p => p.CustomerId == e.CustomerId, cancellationToken);
        if (prefs is not null)
        {
            prefs.EmailNotificationsEnabled = e.EmailNotificationsEnabled;
            prefs.SmsNotificationsEnabled = e.SmsNotificationsEnabled;
        }

        var version = await _dbContext.CustomerChanges
            .Where(c => c.CustomerId == e.CustomerId)
            .OrderByDescending(c => c.Version)
            .Select(c => c.Version)
            .FirstOrDefaultAsync(cancellationToken) + 1;

        _dbContext.CustomerChanges.Add(new CustomerChangeReadModel
        {
            Id = Guid.NewGuid(),
            CustomerId = e.CustomerId,
            Version = version,
            Type = nameof(CustomerPreferencesUpdatedDomainEvent),
            OccurredOnUtc = e.OccurredOnUtc
        });

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

        var prefs = await _dbContext.CustomerNotificationPreferences.FirstOrDefaultAsync(p => p.CustomerId == e.CustomerId, cancellationToken);
        if (prefs is not null)
        {
            _dbContext.CustomerNotificationPreferences.Remove(prefs);
        }

        var version = await _dbContext.CustomerChanges
            .Where(c => c.CustomerId == e.CustomerId)
            .OrderByDescending(c => c.Version)
            .Select(c => c.Version)
            .FirstOrDefaultAsync(cancellationToken) + 1;

        _dbContext.CustomerChanges.Add(new CustomerChangeReadModel
        {
            Id = Guid.NewGuid(),
            CustomerId = e.CustomerId,
            Version = version,
            Type = nameof(CustomerDeletedDomainEvent),
            OccurredOnUtc = e.OccurredOnUtc
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
