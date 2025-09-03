using Domain.Primitives;
using Modules.Customer.Domain.Entities;

namespace Modules.Customer.Domain.Abstractions;

public interface ICustomerEventStore
{
    Task AppendAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IDomainEvent>> LoadAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StoredEvent>> GetHistoryAsync(Guid customerId, CancellationToken cancellationToken = default);
}

