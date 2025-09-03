namespace Modules.Customer.Domain.Abstractions;

public sealed record StoredEvent(string Type, string Data, long Version, DateTimeOffset OccurredOnUtc);

