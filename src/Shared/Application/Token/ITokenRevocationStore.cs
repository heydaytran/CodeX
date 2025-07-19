namespace Application.Token;

public interface ITokenRevocationStore
{
    Task RevokeAsync(string jti, TimeSpan expiresIn);
    Task<bool> IsRevokedAsync(string jti);
}