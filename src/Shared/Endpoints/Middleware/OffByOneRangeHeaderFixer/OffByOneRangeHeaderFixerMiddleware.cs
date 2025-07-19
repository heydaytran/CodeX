namespace Endpoints.Middleware.OffByOneRangeHeaderFixer;

public class OffByOneRangeHeaderFixerMiddleware
{
    private readonly RequestDelegate _next;

    public OffByOneRangeHeaderFixerMiddleware(RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(next);

        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var enabled = context
            .GetEndpoint()?
            .Metadata?
            .GetMetadata<OffByOneRangeHeaderFixerMetadata>()?.Enabled;

        if (enabled is null || !enabled(context.Request))
        {
            await _next(context);
            return;
        }

        var incorrectRangeHeaders = new RequestHeaders(context.Request.Headers).Range;
        if (incorrectRangeHeaders is null || incorrectRangeHeaders.Ranges.Count == 0)
        {
            await _next(context);
            return;
        }

        RangeHeaderValue fixedRangeHeaderValue = new()
        {
            Unit = incorrectRangeHeaders.Unit,
        };

        // Due to an off by one error in current comms that we can't fix due to backwards compatibility
        // adjust the range here and write back to the headers.
        long? oldTo = null, newTo = null;
        foreach (var range in incorrectRangeHeaders.Ranges)
        {
            if (range is null || range.From is null || range.To is null || range.To <= 0)
            {
                continue;
            }

            oldTo = range.To;
            newTo = range.To - 1;
            fixedRangeHeaderValue.Ranges.Add(new RangeItemHeaderValue(range.From, newTo));
        }

        context.Request.Headers.Range = fixedRangeHeaderValue.ToString();

        context.Response.OnStarting(() =>
        {
            if (oldTo is null)
            {
                return Task.CompletedTask;
            }

            ResponseHeaders responseHeaders = new(context.Response.Headers);
            var contentRange = responseHeaders.ContentRange;
            if (contentRange is null || !contentRange.HasRange)
            {
                return Task.CompletedTask;
            }

            long? to = contentRange.To;
            long? from = contentRange.From;
            if (to == null || from == null)
            {
                return Task.CompletedTask;
            }

            ContentRangeHeaderValue fixedContentRange = contentRange.HasLength
                ? new(from.Value, Math.Min(oldTo.Value, contentRange.Length - 1 ?? 0), contentRange.Length ?? 0)
                : new(from.Value, oldTo.Value);

            context.Response.Headers.ContentRange = fixedContentRange.ToString();

            return Task.CompletedTask;
        });

        await _next(context);
    }
}