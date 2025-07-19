using Infrastructure.Extensions;

namespace Endpoints.Middleware.MonitorResponse;

public static class MonitorResponseMiddlewareExtensions
{
    public static IApplicationBuilder UseMonitorResponse(this IApplicationBuilder builder) => 
        builder.UseMiddleware<MonitorResponseMiddleware>(Options.Create(new MonitorResponseMiddlewareOptions()));

    public static TBuilder WithMonitorResponse<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder => 
        builder
            .Tap(b => b.Add(endpointBuilder =>
            {
                if (!endpointBuilder.Metadata.Contains(MonitorResponseMetadata.Instance))
                {
                    endpointBuilder.Metadata.Add(MonitorResponseMetadata.Instance);
                }
            }));
}