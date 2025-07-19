namespace Infrastructure.Caching;

internal sealed class RedisOutputCacheOptionsSetup(IOptions<RedisConnectionStringOptions> connectionStringOptions) :
    IConfigureOptions<RedisOutputCacheOptions>
{
    private readonly IOptions<RedisConnectionStringOptions> _connectionStringOptions = connectionStringOptions;

    public void Configure(RedisOutputCacheOptions options)
    {
        options.Configuration = _connectionStringOptions.Value;
        options.InstanceName = "Lsg.Nexus.Api";
    }
}