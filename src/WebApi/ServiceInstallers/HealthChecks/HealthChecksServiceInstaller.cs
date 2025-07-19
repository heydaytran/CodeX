namespace WebApi.ServiceInstallers.HealthChecks;

internal sealed class HealthChecksServiceInstaller : IServiceInstaller
{
    private const string SqlServerConnectionStringName = "Configurations";
    private const string RedConnectionStringName = "Redis";
    private static readonly string[] SqlServerHealthCheckTags = ["db", "sql", "postgres"];
    private static readonly string[] RedisHealthCheckTags = ["redis", "caching"];
    private static readonly string[] ApplicationInsigntsHealthCheckTags = ["applicationinsights", "logging", "diagnostics"];

    /// <inheritdoc/>
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var sqlServerConnectionString = configuration.GetConnectionString(SqlServerConnectionStringName);
        var redisConnectionString = configuration.GetConnectionString(RedConnectionStringName) ?? "localhost";
        var travellerConnectionString = configuration.GetConnectionString("Traveller");
        var healthChecks = services
            .AddHealthChecks()
            .AddNpgSql(
                connectionString: sqlServerConnectionString ?? string.Empty,
                healthQuery: "SELECT 1;",
                name: "postgres",
                failureStatus: HealthStatus.Unhealthy,
                tags: SqlServerHealthCheckTags)
            .AddRedis(
                redisConnectionString: redisConnectionString,
                name: "redis",
                failureStatus: HealthStatus.Unhealthy,
                tags: RedisHealthCheckTags)
            .AddSqlServer(
                connectionString: travellerConnectionString ?? string.Empty,
                healthQuery: "SELECT 1;",
                name: "sql",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["db", "sql", "sqlserver", "traveller"])
            .AddApplicationInsightsPublisher();
    }
}