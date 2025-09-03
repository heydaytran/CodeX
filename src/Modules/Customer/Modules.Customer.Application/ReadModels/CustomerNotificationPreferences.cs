namespace Modules.Customer.Application.ReadModels;

public sealed record CustomerNotificationPreferences(
    Guid Id,
    bool EmailNotificationsEnabled,
    bool SmsNotificationsEnabled);

