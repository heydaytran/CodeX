using Endpoints.Middleware.AddResponseHeader;
using Infrastructure.Configuration;
using Infrastructure.Extensions;

namespace Endpoints;

public class EndpointServiceInstaller : IServiceInstaller
{
    /// <inheritdoc/>
    public void Install(IServiceCollection services, IConfiguration configuration) 
        => services
            .ConfigureOptions<AddResponseHeaderOptionsSetup>()
            .AddSingletonAsSelfWithInterfaces(AssemblyReference.Assembly)
            .AddControllers();
}