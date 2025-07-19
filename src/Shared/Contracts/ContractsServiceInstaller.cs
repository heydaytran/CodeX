using Infrastructure.Configuration;
using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Contracts;

public class ContractsServiceInstaller : IServiceInstaller
{
    /// <inheritdoc/>
    public void Install(IServiceCollection services, IConfiguration configuration)
        => services
            .AddSingletonAsSelfWithInterfaces(AssemblyReference.Assembly);
}