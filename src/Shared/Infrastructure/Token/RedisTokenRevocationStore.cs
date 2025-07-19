using Application.Token;
using StackExchange.Redis;

namespace Infrastructure.Token;

public class RedisTokenRevocationStore : ITokenRevocationStore
{
    private readonly IDatabase _db;
    private const string Prefix = "revoked-jti:";

    public RedisTokenRevocationStore(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public Task RevokeAsync(string jti, TimeSpan expiresIn)
    {
        return _db.StringSetAsync(Prefix + jti, "revoked", expiresIn);
    }

    public async Task<bool> IsRevokedAsync(string jti)
    {
        return await _db.KeyExistsAsync(Prefix + jti);
    }
}
