using Endpoints.Extensions;

namespace Endpoints.CustomResults;

public class ZipContentResult<T>(T content, IZipArchiveMaker<T> archiveMaker, bool enableRangeProcessing = false, bool surpressRangeNotSatisfiable = false) : IResult
{
    private readonly T _content = content ??
        throw new ArgumentNullException(nameof(content));

    private readonly IZipArchiveMaker<T> _archiveMaker = archiveMaker ??
        throw new ArgumentNullException(nameof(archiveMaker));

    private readonly bool _enableRangeProcessing = enableRangeProcessing;
    private readonly bool _surpressRangeNotSatisfiable = surpressRangeNotSatisfiable;

    /// <inherit />
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        MemoryStream zipStream = new();
        using ZipArchive archive = new(zipStream, ZipArchiveMode.Create, true);

        await _archiveMaker.WriteToAsync(archive, _content, default);
        archive.Dispose();

        zipStream.Position = 0;

        bool range = _enableRangeProcessing && httpContext.Request.IsTicketerRangeRequestEnabled();
        if (_surpressRangeNotSatisfiable && range)
        {
            CapRangeToContentLength(httpContext, zipStream.Length);
        }

        var fileResult = Results.File(
            zipStream, 
            MediaTypeNames.Application.Zip,
            enableRangeProcessing: range);
        await fileResult.ExecuteAsync(httpContext);
    }

    private static bool CapRangeToContentLength(HttpContext context, long contentLength)
    {
        if (RangeSatisfiable(context, contentLength))
        {
            return false;
        }

        if (contentLength <= 0)
        {
            return false;
        }

        if (context.Request.IsTicketerRangeRequestV1())
        {
            // V1, returns the entire file so remove the range header.
            context.Request.Headers.Remove(HeaderNames.Range);

            return true;
        }

        // V2, returns zero bytes but with content range set to the entire file.
        var range = new RangeHeaderValue(0, 0);

        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Remove(HeaderNames.ContentRange);
            context.Response.Headers.Append(
                HeaderNames.ContentRange,
                new ContentRangeHeaderValue(contentLength - 1, contentLength - 1, contentLength).ToString());

            context.Response.StatusCode = StatusCodes.Status206PartialContent;
            return Task.CompletedTask;
        });

        return true;
    }

    private static bool RangeSatisfiable(HttpContext context, long contentLength)
    {
        var rangeHeaders = new RequestHeaders(context.Request.Headers).Range;
        if (rangeHeaders?.Ranges is null || !(rangeHeaders.Ranges?.Count > 0))
        {
            return true;
        }

        var range = rangeHeaders.Ranges.First();
        return range.From is null || range.From < contentLength;
    }
}