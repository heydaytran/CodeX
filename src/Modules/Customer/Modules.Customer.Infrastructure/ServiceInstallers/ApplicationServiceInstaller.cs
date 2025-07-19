using Modules.Customer.Application.Mapper;

namespace Modules.Customer.Infrastructure.ServiceInstallers;

public sealed class ApplicationServiceInstaller : IServiceInstaller
{
    /// <inheritdoc />
    public void Install(IServiceCollection services, IConfiguration configuration) =>
        services
            .Tap(services.TryAddTransient<IEventBus, EventBus>)
            .AddValidatorsFromAssembly(Application.AssemblyReference.Assembly)
            .AddScopedAsMatchingInterfaces(Application.AssemblyReference.Assembly)
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Application.AssemblyReference.Assembly);
                cfg.AddBehavior(
                    typeof(IPipelineBehavior<,>),
                    typeof(CachingBehavior<,>));
                cfg.AddBehavior(
                    typeof(IPipelineBehavior<,>),
                    typeof(LoggingBehaviour<,>));
                cfg.AddBehavior(
                    typeof(IPipelineBehavior<,>),
                    typeof(ValidationBehaviour<,>));
            })
            .AddScoped<ICustomerMapper, CustomerMapper>();
}