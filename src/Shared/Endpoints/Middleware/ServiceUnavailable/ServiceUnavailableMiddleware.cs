namespace Endpoints.Middleware.ServiceUnavailable;

public sealed class ServiceUnavailableMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next ??
        throw new ArgumentNullException(nameof(next));

    public Task InvokeAsync(HttpContext context, IOptionsSnapshot<ServiceUnavailableMiddlewareOptions> options)
    {
        if (!options.Value.Enabled)
        {
            return _next(context);
        }

        ServiceUnavailableMetadata? metadata = context
            .GetEndpoint()?
            .Metadata?
            .GetMetadata<ServiceUnavailableMetadata>();

        if (metadata is null)
        {
            return _next(context);
        }

        if (!metadata.Condition(context))
        {
            return _next(context);
        }

        context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;

        metadata.WriteResponseAsync?.Invoke(context.Response);

        return Task.CompletedTask;
    }
}
