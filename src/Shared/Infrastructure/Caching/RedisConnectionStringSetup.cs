namespace Infrastructure.Caching;

internal sealed class RedisConnectionStringSetup(IConfiguration configuration) : IConfigureOptions<RedisConnectionStringOptions>
{
    private const string ConnectionStringName = "Redis";

    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    /// <inheritdoc />
    public void Configure(RedisConnectionStringOptions options) => 
        options.Value = 
        _configuration.GetConnectionString(ConnectionStringName) ?? throw new Exception("Connection string not found.");
}