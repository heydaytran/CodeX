using Domain.Primitives;

namespace Authentication.Entities;

public class RefreshToken : Entity<Guid>
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = default!;
    public DateTime Expires { get; set; }
    public bool Revoked { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = default!;
}