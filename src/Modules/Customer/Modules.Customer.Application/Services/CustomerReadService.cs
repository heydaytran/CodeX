using System.Linq;
using Application.Lifetimes;
using Microsoft.EntityFrameworkCore;
using Modules.Customer.Application.ReadModels;
using Modules.Customer.Persistence;

namespace Modules.Customer.Application.Services;

public sealed class CustomerReadService(CustomerReadDbContext dbContext) : ICustomerReadService, ITransient
{
    private readonly CustomerReadDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<CustomerProfile?> GetProfileAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var profile = await _dbContext.CustomerProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == customerId, cancellationToken);
        if (profile is null)
        {
            return null;
        }

        return new CustomerProfile(
            profile.Id,
            profile.Title,
            profile.FirstName,
            profile.LastName,
            profile.Email,
            profile.EmailNotificationsEnabled,
            profile.SmsNotificationsEnabled);
    }

    public async Task<IReadOnlyList<CustomerChange>> GetChangeHistoryAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var history = await _dbContext.CustomerChanges
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId)
            .OrderBy(c => c.Version)
            .ToListAsync(cancellationToken);
        return history.Select(e => new CustomerChange(e.Version, e.Type, e.OccurredOnUtc)).ToList();
    }

    public async Task<CustomerNotificationPreferences?> GetNotificationPreferencesAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var prefs = await _dbContext.CustomerNotificationPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.CustomerId == customerId, cancellationToken);
        if (prefs is null)
        {
            return null;
        }

        return new CustomerNotificationPreferences(
            prefs.CustomerId,
            prefs.EmailNotificationsEnabled,
            prefs.SmsNotificationsEnabled);
    }
}

