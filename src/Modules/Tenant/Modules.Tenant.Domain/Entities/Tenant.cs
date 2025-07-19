using Domain.Primitives;

namespace Modules.Customer.Domain.Entities;

public sealed class Tenant : Entity<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Branding { get; set; } = string.Empty; // JSON string
    public bool IsActive { get; set; } = true;

}
