namespace WebApi.ServiceInstallers.Swagger;

internal sealed class SwaggerServiceInstaller : IServiceInstaller
{
    /// <inheritdoc/>
    public void Install(IServiceCollection services, IConfiguration configuration) => 
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
}