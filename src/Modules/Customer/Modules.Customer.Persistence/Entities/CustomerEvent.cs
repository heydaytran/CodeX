namespace Modules.Customer.Persistence.Entities;

public sealed class CustomerEvent
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public long Version { get; set; }
    public DateTimeOffset OccurredOnUtc { get; set; }
}

