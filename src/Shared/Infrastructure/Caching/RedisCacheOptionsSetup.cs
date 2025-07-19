namespace Infrastructure.Caching;

internal sealed class RedisCacheOptionsSetup(IOptions<RedisConnectionStringOptions> connectionStringOptions) : 
    IConfigureOptions<RedisCacheOptions>
{
    private readonly IOptions<RedisConnectionStringOptions> _connectionStringOptions = connectionStringOptions;

    public void Configure(RedisCacheOptions options)
    {
        options.Configuration = _connectionStringOptions.Value;
        options.InstanceName = "Api.Template";
    }
}