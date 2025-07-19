namespace Persistence.Extensions;

public static class IPrincipalExtensions
{
    public static Guid? UserId(this IPrincipal principal)
    {
        var claims = principal as ClaimsPrincipal;
        var value = claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (value == null)
        {
            return null;
        }

        if (!Guid.TryParse(value, out var id))
        {
            return null;
        }

        return id;
    }
}
