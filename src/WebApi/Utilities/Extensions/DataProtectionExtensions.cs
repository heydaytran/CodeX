using Authentication.Database;

namespace WebApi.Utilities.Extensions;

internal static class DataProtectionExtensions
{
    internal static void AddDataProtection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthenticationDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("Identity"))
                .AddInterceptors(new SoftDeleteInterceptor()));
        services.AddDataProtection()
            .PersistKeysToDbContext<AuthenticationDbContext>()
            .UseCryptographicAlgorithms(
                new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                })
            .SetApplicationName(configuration["ApplicationName"] ?? "MinimalApi");
    }
}