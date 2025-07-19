using Authentication.Entities;
using Microsoft.AspNetCore.Identity;

namespace Modules.Identity.Domain.Auth;

public interface ITokenService
{
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    Task<IdentityUserToken<string>?> GetUserTokenAsync(string UserId, CancellationToken cancellation);
}