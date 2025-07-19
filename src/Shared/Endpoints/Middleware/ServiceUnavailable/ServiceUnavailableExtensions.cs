using Infrastructure.Extensions;

namespace Endpoints.Middleware.ServiceUnavailable;

public static class ServiceUnavailableExtensions
{
    public static IApplicationBuilder UseServiceUnavailable(this IApplicationBuilder builder) => 
        builder.UseMiddleware<ServiceUnavailableMiddleware>();

    public static TBuilder WithServiceUnavailable<TBuilder>(this TBuilder builder, ServiceUnavailableMetadata metadata) where TBuilder : IEndpointConventionBuilder => 
        builder
            .Tap(b => b.Add(endpointBuilder =>
            {
                if (!endpointBuilder.Metadata.Contains(metadata))
                {
                    endpointBuilder.Metadata.Add(metadata);
                }
            }));
}