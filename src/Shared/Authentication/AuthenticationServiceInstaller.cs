using Application.Behaviours;
using Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Interceptors;
using Persistence.Options;

namespace Authentication;

internal sealed class AuthenticationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration) =>
        services
            .AddDbContext<AuthenticationDbContext>((serviceProvider, options) =>
            {
                var connectionString = serviceProvider
                    .GetRequiredService<IOptions<ConnectionStringOptions>>().Value;

                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.UseRelationalNulls();
                }).AddInterceptors(new SoftDeleteInterceptor());
            })
            .AddTransient<IAuthenticationRepository, AuthenticationRepository>()
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly);
                cfg.AddBehavior(
                    typeof(IPipelineBehavior<,>),
                    typeof(CachingBehavior<,>));
                cfg.AddBehavior(
                    typeof(IPipelineBehavior<,>),
                    typeof(LoggingBehaviour<,>));
            });
}