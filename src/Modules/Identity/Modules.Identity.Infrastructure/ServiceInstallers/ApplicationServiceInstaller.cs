using Application.Behaviours;
using Application.Behaviours.Caching;
using Application.EventBus;
using Infrastructure.EventBus;
using Modules.Identity.Domain.Auth;
using Modules.Identity.Infrastructure.Auth;

namespace Modules.Identity.Infrastructure.ServiceInstallers;

public sealed class ApplicationServiceInstaller : IServiceInstaller
{
    /// <inheritdoc />
    public void Install(IServiceCollection services, IConfiguration configuration) =>
        services
            .Tap(services.TryAddTransient<IEventBus, EventBus>)
            .AddScoped<ITokenService, TokenService>()
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
            });
}