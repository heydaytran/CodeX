namespace Infrastructure.Caching;

public sealed class RedisConnectionStringOptions
{
    public string Value { get; internal set; } = string.Empty;

    public static implicit operator string(RedisConnectionStringOptions connectionString) => connectionString.Value;
}