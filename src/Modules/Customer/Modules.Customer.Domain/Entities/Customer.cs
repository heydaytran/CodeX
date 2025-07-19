using Domain.Primitives;
using Domain.ValueObjects;
using System.Xml.Linq;

namespace Modules.Customer.Domain.Entities;
public sealed class Customer : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public ContactInformation ContactInformation { get; set; } = null!;
  
}