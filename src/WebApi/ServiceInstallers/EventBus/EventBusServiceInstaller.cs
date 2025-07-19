namespace WebApi.ServiceInstallers.EventBus;

internal sealed class EventBusServiceInstaller : IServiceInstaller
{
    /// <inheritdoc />
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();
        
        services
            .ConfigureOptions<MassTransitHostOptionsSetup>()
            .AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();
                // busConfigurator.AddConsumersFromAssemblies(
                //     Modules.Checkouts.Infrastructure.AssemblyReference.Assembly);
                // Inject RabbitMQ settings into bus configuration
                if (rabbitMqSettings?.Username is null || rabbitMqSettings?.Password is null)
                {
                    busConfigurator.UsingInMemory((context, configurator) =>
                    {
                        configurator.ConfigureEndpoints(context);
                    });
                }
                else
                {
                    busConfigurator.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                        {
                            h.Username(rabbitMqSettings.Username);
                            h.Password(rabbitMqSettings.Password);
                        });
                        cfg.ConfigureEndpoints(context);
                    });
                }
            });
    }
        
}