using Endpoints.Extensions;

namespace Endpoints.Middleware.StatusCodeWhenHeaderExists;

public sealed class StatusCodeWhenHeaderExistsMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next ??
        throw new ArgumentNullException(nameof(next));

    public Task InvokeAsync(HttpContext context)
    {
        StatusCodeWhenHeaderExistsMetadata? metadata = context
            .GetEndpoint()?
            .Metadata?
            .GetMetadata<StatusCodeWhenHeaderExistsMetadata>();

        if (metadata is null)
        {
            return _next(context);
        }

        var headerValue = context.Request.QueryOrForm(metadata.HeaderName);
        if (string.IsNullOrEmpty(headerValue))
        {
            return _next(context);
        }

        context.Response.StatusCode = (int)metadata.StatusCode;

        return Task.CompletedTask;
    }
}
