namespace WebApi.ServiceInstallers.Caching;

internal sealed class CachingServiceInstaller : IServiceInstaller
{
    /// <inheritdoc/>
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddStackExchangeRedisOutputCache(_ => { })
            .AddStackExchangeRedisCache(_ => { })
            .AddSingleton<IConnectionMultiplexer>(services =>
            {
                var connectionString = services.GetRequiredService<IOptions<RedisConnectionStringOptions>>().Value;

                return ConnectionMultiplexer.Connect(connectionString);
            });
    }
}