using Endpoints.Extensions;

namespace Endpoints.Middleware.MonitorResponse;

public sealed class MonitorResponseMiddleware(RequestDelegate next, IOptions<MonitorResponseMiddlewareOptions> options)
{
    private readonly RequestDelegate _next = next ?? 
        throw new ArgumentNullException(nameof(next));

    private readonly MonitorResponseMiddlewareOptions _options = options?.Value ??
        throw new ArgumentNullException(nameof(next));

    public Task InvokeAsync(HttpContext context)
    {
        var enabled = context
            .GetEndpoint()?
            .Metadata?
            .GetMetadata<MonitorResponseMetadata>() is not null;
        if (!enabled)
        {
            return _next(context);
        }

        var headerName = _options.HeaderName;
        var responseBody = _options.ResponseContent;

        var headerValue = context.Request.QueryOrForm(headerName);
        var hasHeader = !string.IsNullOrEmpty(headerValue);
        if (!hasHeader)
        {
            return _next(context);
        }

        return context.Response.WriteAsync(responseBody); 
    }
}