using Modules.Configuration.Migrator;

namespace Modules.Configuration.Infrastructure;

public class ConfigurationsModuleInstaller : IModuleInstaller
{
    /// <inheritdoc/>
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        new MigrateDatabase().Execute(configuration);
        services
            .InstallServicesFromAssemblies(configuration, AssemblyReference.Assembly)
            .AddScopedAsMatchingInterfaces(AssemblyReference.Assembly);
    }
}