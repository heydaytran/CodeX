using Domain.Primitives;

namespace Modules.Customer.Domain.Events;

public sealed record CustomerPreferencesUpdatedDomainEvent(
    Guid CustomerId,
    bool EmailNotificationsEnabled,
    bool SmsNotificationsEnabled
) : DomainEvent(DateTimeOffset.UtcNow);

