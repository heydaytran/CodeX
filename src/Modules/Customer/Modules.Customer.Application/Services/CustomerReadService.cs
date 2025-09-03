using System.Linq;
using Application.Lifetimes;
using Modules.Customer.Application.ReadModels;
using Modules.Customer.Domain.Abstractions;
using Modules.Customer.Domain.Entities;

namespace Modules.Customer.Application.Services;

public sealed class CustomerReadService(ICustomerEventStore eventStore) : ICustomerReadService, ITransient
{
    private readonly ICustomerEventStore _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));

    public async Task<CustomerProfile?> GetProfileAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var events = await _eventStore.LoadAsync(customerId, cancellationToken);
        if (events.Count == 0)
        {
            return null;
        }

        var customer = Customer.Rehydrate(events);

        return new CustomerProfile(
            customer.Id,
            customer.Title,
            customer.FirstName,
            customer.LastName,
            customer.ContactInformation.Email,
            customer.EmailNotificationsEnabled,
            customer.SmsNotificationsEnabled);
    }

    public async Task<IReadOnlyList<CustomerChange>> GetChangeHistoryAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var history = await _eventStore.GetHistoryAsync(customerId, cancellationToken);
        return history.Select(e => new CustomerChange(e.Version, e.Type, e.OccurredOnUtc)).ToList();
    }

    public async Task<CustomerNotificationPreferences?> GetNotificationPreferencesAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var events = await _eventStore.LoadAsync(customerId, cancellationToken);
        if (events.Count == 0)
        {
            return null;
        }

        var customer = Customer.Rehydrate(events);

        return new CustomerNotificationPreferences(
            customer.Id,
            customer.EmailNotificationsEnabled,
            customer.SmsNotificationsEnabled);
    }
}

