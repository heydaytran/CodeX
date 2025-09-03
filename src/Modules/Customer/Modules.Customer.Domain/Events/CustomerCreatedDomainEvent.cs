using Domain.Primitives;
using Modules.Customer.Domain.Entities;

namespace Modules.Customer.Domain.Events;

public sealed record CustomerCreatedDomainEvent(
    Guid CustomerId,
    Guid TenantId,
    Guid UserId,
    string Title,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Gender,
    ContactInformation ContactInformation,
    bool EmailNotificationsEnabled,
    bool SmsNotificationsEnabled
) : DomainEvent(DateTimeOffset.UtcNow);

