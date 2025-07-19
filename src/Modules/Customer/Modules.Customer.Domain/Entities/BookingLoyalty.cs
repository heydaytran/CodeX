using Domain.Primitives;
namespace Modules.Customer.Domain.Entities;

public sealed class BookingLoyalty : Entity<Guid>
{
    public Guid CustomerId { get; set; }
    public DateTime? FirstBookingDate { get; set; }
    public DateTime? LastBookingDate { get; set; }
    public int BookingCount { get; set; }
    public decimal TotalSpent { get; set; }
    public int LoyaltyPointsBalance { get; set; }
}

