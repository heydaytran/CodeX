namespace Endpoints.Middleware.AddResponseHeader;

public sealed class AddResponseHeaderMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AddResponseHeaderOptions _options;

    public AddResponseHeaderMiddleware(RequestDelegate next, IOptions<AddResponseHeaderOptions> options)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(options);

        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var metadata = context.GetEndpoint()?.Metadata;

        var headers = GetResponseHeaders(context);

        context.Response.OnStarting(() =>
        {
            foreach (var header in headers)
            {
                var value = header.GetValue(context.Response);
                if (value is null)
                {
                    continue;
                }

                context.Response.Headers.Append(header.Key, value.Value);
            }

            return Task.CompletedTask;
        });

        await _next(context);
    }

    private List<ResponseHeaderMetadata> GetResponseHeaders(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        List<ResponseHeaderMetadata> headers = [];

        if (_options.BaseHeaders != null)
        {
            headers.AddRange(_options.BaseHeaders);
        }

        var metadata = context.GetEndpoint()?.Metadata;

        var endpointHeaders = metadata?.GetOrderedMetadata<ResponseHeaderMetadata>();

        if (endpointHeaders != null)
        {
            endpointHeaders = endpointHeaders.Where(h => h.Condition is null || h.Condition(context)).ToList();

            headers.AddRange(endpointHeaders);
        }

        return headers;
    }
}
