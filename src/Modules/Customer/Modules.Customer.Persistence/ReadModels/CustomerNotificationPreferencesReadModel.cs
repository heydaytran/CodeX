using System.ComponentModel.DataAnnotations;

namespace Modules.Customer.Persistence.ReadModels;

public class CustomerNotificationPreferencesReadModel
{
    [Key]
    public Guid CustomerId { get; set; }
    public bool EmailNotificationsEnabled { get; set; }
    public bool SmsNotificationsEnabled { get; set; }
}
