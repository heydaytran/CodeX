namespace WebApi.Utilities.Logging.Extensions;

internal static class LoggerEnrichmentConfigurationExtensions
{
    public static LoggerConfiguration WithRequestHeaders(
       this LoggerEnrichmentConfiguration enrichmentConfiguration, string[]? exclude = null) => 
        enrichmentConfiguration.With(new RequestHeadersEnricher(exclude));
}