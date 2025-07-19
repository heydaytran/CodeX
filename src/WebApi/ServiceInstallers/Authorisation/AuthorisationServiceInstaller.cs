namespace WebApi.ServiceInstallers.Authorisation;

internal sealed class AuthorisationServiceInstaller : IServiceInstaller
{
    /// <inheritdoc/>
    public void Install(IServiceCollection services, IConfiguration configuration) =>
        services
            .AddAuthorization(options =>
            {
                options.AddPolicy(
                    Policies.OnlyCustomers,
                    policy => policy.Requirements.Add(new OnlyCustomersRequirement())
                );
                options.AddPolicy(Policies.OnlyAdmins, policy => policy.Requirements.Add(new OnlyAdminsRequirement()));
                options.AddPolicy(
                    Policies.OnlyThirdParties,
                    policy => policy.Requirements.Add(new OnlyThirdPartiesRequirement())
                );
            })
            .AddSingleton<IAuthorizationHandler, OnlyCustomersAuthorizationHandler>()
            .AddSingleton<IAuthorizationHandler, OnlyAdminsAuthorizationHandler>()
            .AddSingleton<IAuthorizationHandler, OnlyThirdPartiesAuthorizationHandler>();

}
