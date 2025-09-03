using Domain.Primitives;

namespace Modules.Customer.Domain.Events;

public sealed record CustomerDeletedDomainEvent(
    Guid CustomerId
) : DomainEvent(DateTimeOffset.UtcNow);

