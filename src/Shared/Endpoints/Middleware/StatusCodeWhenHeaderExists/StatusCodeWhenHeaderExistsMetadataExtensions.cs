using Infrastructure.Extensions;

namespace Endpoints.Middleware.StatusCodeWhenHeaderExists;

public static class StatusCodeWhenHeaderExistsMetadataExtensions
{
    public static IApplicationBuilder UseStatusCodeWhenHeaderExists(this IApplicationBuilder builder)
        => builder.UseMiddleware<StatusCodeWhenHeaderExistsMiddleware>();

    public static TBuilder WithStatusCodeWhenHeaderExists<TBuilder>(this TBuilder builder, StatusCodeWhenHeaderExistsMetadata metadata) where TBuilder : IEndpointConventionBuilder =>
        builder
            .Tap(b => b.Add(endpointBuilder =>
            {
                if (!endpointBuilder.Metadata.Contains(metadata))
                {
                    endpointBuilder.Metadata.Add(metadata);
                }
            }));
}