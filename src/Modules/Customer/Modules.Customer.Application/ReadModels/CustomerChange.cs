namespace Modules.Customer.Application.ReadModels;

public sealed record CustomerChange(long Version, string Type, DateTimeOffset OccurredOnUtc);

