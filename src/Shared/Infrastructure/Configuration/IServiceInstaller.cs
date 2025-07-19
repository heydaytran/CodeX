namespace Infrastructure.Configuration;

public interface IServiceInstaller
{
    /// <summary>
    /// Installs the required services using the specified service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    void Install(IServiceCollection services, IConfiguration configuration);
}