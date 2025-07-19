using Domain.Extensions;

namespace Endpoints.CustomResults;

public class GpxContentResult<T>(T content, IGpxConverter<T> converter) : IResult
{
    private const string GpxContentType = "application/gpx+xml";

    private readonly T _content = content ??
        throw new ArgumentNullException(nameof(content));

    private readonly IGpxConverter<T> _converter = converter ??
        throw new ArgumentNullException(nameof(converter));

    /// <inherit />
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var gpxDocument = _converter.Convert(_content);
        var gpxString = gpxDocument.ToString(true);

        httpContext.Response.ContentType = GpxContentType;
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(gpxString);

        await httpContext.Response.WriteAsync(gpxString);
    }
}