namespace Persistence.Options;

internal sealed class ConnectionStringSetup(IConfiguration configuration, ILogger<ConnectionStringSetup> logger) : IConfigureOptions<ConnectionStringOptions>
{
    private const string ConnectionStringName = "Configurations";
    private readonly IConfiguration _configuration = configuration ?? 
        throw new ArgumentNullException(nameof(logger));

    private readonly ILogger<ConnectionStringSetup> _logger = logger ?? 
        throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public void Configure(ConnectionStringOptions options)
    {
        options.Value = _configuration.GetConnectionString(ConnectionStringName) ?? 
            throw new Exception("Connection string not found.");

        _logger.LogInformation("Database connection string. {DatabaseConnectionString}", options);
    }
}