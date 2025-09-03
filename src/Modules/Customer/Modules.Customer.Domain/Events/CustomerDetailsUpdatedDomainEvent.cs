using Domain.Primitives;
using Modules.Customer.Domain.Entities;

namespace Modules.Customer.Domain.Events;

public sealed record CustomerDetailsUpdatedDomainEvent(
    Guid CustomerId,
    string Title,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Gender,
    ContactInformation ContactInformation
) : DomainEvent(DateTimeOffset.UtcNow);

