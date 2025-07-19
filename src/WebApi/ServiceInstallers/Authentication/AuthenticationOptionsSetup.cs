namespace WebApi.ServiceInstallers.Authentication;

internal sealed class AuthenticationOptionsSetup : IConfigureOptions<AuthenticationOptions>
{
    /// <inheritdoc />
    public void Configure(AuthenticationOptions options) => options.DefaultScheme = ApiKeyAuthenticationOptions.DefaultScheme;
}