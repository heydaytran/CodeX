using Domain.Primitives;


namespace Modules.Customer.Domain.Entities;

public sealed class ContactInformation : Entity<Guid>
{
    public Guid CustomerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string County { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public Customer Customer { get; set; } = null!;

}

