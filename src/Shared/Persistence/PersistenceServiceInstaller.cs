using Persistence.Options;
using Infrastructure.Configuration;
using Infrastructure.Extensions;

namespace Persistence;

public class PersistenceServiceInstaller : IServiceInstaller
{
    /// <inheritdoc>
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services
            .ConfigureOptions<ConnectionStringSetup>()
            .AddSingletonAsSelfWithInterfaces(AssemblyReference.Assembly);
    }
}