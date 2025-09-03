namespace Modules.Customer.Persistence.ReadModels;

public class CustomerChangeReadModel
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public long Version { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset OccurredOnUtc { get; set; }
}
