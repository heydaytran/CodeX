

namespace Modules.Identity.Infrastructure.Auth;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly AuthenticationDbContext _identityDbContext;


    public TokenService(IConfiguration config,
        AuthenticationDbContext authenticationDbContext)
    {
        _config = config;
        _identityDbContext =authenticationDbContext;
    }

    public string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jti = Guid.NewGuid().ToString();
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, jti)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public async Task<IdentityUserToken<string>?> GetUserTokenAsync(string UserId, CancellationToken cancellation)
    {
        var userToken = await _identityDbContext.UserTokens
                      .Where(ut => ut.UserId == UserId && ut.Name == "RefreshToken")
                      .FirstOrDefaultAsync(cancellation) ;
        return userToken;
    }
}
