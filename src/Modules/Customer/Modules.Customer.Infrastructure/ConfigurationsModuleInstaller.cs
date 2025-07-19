namespace Modules.Customer.Infrastructure;

public class ConfigurationsModuleInstaller : IModuleInstaller
{
    /// <inheritdoc/>
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        new MigrateDatabase().Execute(configuration);
        services
            .InstallServicesFromAssemblies(configuration, AssemblyReference.Assembly)
            .AddTransientAsMatchingInterfaces(Application.AssemblyReference.Assembly)
            .AddScopedAsMatchingInterfaces(Persistence.AssemblyReference.Assembly)
            .AddSingletonAsSelfWithInterfaces(Endpoints.AssemblyReference.Assembly)
            .AddScopedAsMatchingInterfaces(AssemblyReference.Assembly);
    }
}