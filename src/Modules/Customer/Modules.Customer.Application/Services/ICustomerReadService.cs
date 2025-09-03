using Modules.Customer.Application.ReadModels;

namespace Modules.Customer.Application.Services;

public interface ICustomerReadService
{
    Task<CustomerProfile?> GetProfileAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerChange>> GetChangeHistoryAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<CustomerNotificationPreferences?> GetNotificationPreferencesAsync(Guid customerId, CancellationToken cancellationToken = default);
}

