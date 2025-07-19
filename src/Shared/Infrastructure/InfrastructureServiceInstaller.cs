using Application.Audit;
using Application.Behaviours.Caching;
using Application.Token;
using Infrastructure.Audit;
using Infrastructure.Caching;
using Infrastructure.Configuration;
using Infrastructure.Token;
using StackExchange.Redis;

namespace Infrastructure;

public class InfrastructureServiceInstaller : IServiceInstaller
{
    /// <inheritdoc>
    public void Install(IServiceCollection services, IConfiguration configuration) =>
        services
            .AddOutputCache(o =>
            {
                o.MaximumBodySize = 1024 * 1024 * 100; // 100MB
            })
            .AddScoped(typeof(ICacheKeyGenerator<>), typeof(DefaultCacheKeyGenerator<>))
            .AddScoped<IAuditLogger, AuditLogger>()
            .AddScoped<ITokenRevocationStore, RedisTokenRevocationStore>()
            .AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? string.Empty))
            .ConfigureOptions<RedisConnectionStringSetup>()
            .ConfigureOptions<RedisCacheOptionsSetup>()
            .ConfigureOptions<RedisOutputCacheOptionsSetup>();
}