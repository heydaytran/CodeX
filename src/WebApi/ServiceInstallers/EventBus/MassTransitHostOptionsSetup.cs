namespace WebApi.ServiceInstallers.EventBus;

internal sealed class MassTransitHostOptionsSetup : IConfigureOptions<MassTransitHostOptions>
{
    /// <inheritdoc />
    public void Configure(MassTransitHostOptions options) => options.WaitUntilStarted = true;
    
}