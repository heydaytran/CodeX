namespace WebApi.ServiceInstallers.Configuration;

public class ConfigurationServiceInstaller : IServiceInstaller
{ 
    /// <inheritdoc/>
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var useEfConfigurationString = configuration["UseEFConfiguration"];
        if (!bool.TryParse(useEfConfigurationString, out var useEfConfiguration) || !useEfConfiguration)
        {
            return;
        }

        var builder = configuration as IConfigurationBuilder ?? throw new Exception($"Must be configuration builder.");
        
        builder.AddEfConfiguration<SettingsDbContext>(
            options =>
                options.UseNpgsql(configuration.GetConnectionString("Configurations")),
                valueFormatter: new ConfigurationValueFormatter());
    }
}
