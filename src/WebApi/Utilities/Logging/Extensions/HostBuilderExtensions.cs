namespace WebApi.Utilities.Logging.Extensions;

internal static class HostBuilderExtensions
{
    internal static void UseSerilogWithConfiguration(this IHostBuilder hostBuilder) =>
        hostBuilder.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithCorrelationId()
                .Enrich.WithRequestHeaders(["CK"])
                .Enrich.WithClientIp();
        });
}