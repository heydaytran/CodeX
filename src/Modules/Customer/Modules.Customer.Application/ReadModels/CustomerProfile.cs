namespace Modules.Customer.Application.ReadModels;

public sealed record CustomerProfile(
    Guid Id,
    string Title,
    string FirstName,
    string LastName,
    string Email,
    bool EmailNotificationsEnabled,
    bool SmsNotificationsEnabled);

