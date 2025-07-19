namespace WebApi.ServiceInstallers.Misc;

internal sealed class MiscServiceInstaller : IServiceInstaller
{
    /// <inheritdoc/>
    public void Install(IServiceCollection services, IConfiguration configuration) => 
        services
            .AddHttpContextAccessor()
            .Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            })
            .AddExceptionHandler<GlobalExceptionHandler>();
}
