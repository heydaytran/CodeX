namespace Endpoints.CustomResults;

public class CsvContentResult<T>(T content, ICsvConverter<T> converter) : IResult
{
    private const string ContentType = "text/csv";

    private readonly T _content = content ??
        throw new ArgumentNullException(nameof(content));

    private readonly ICsvConverter<T> _converter = converter ??
        throw new ArgumentNullException(nameof(converter));

    /// <inherit />
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var csvString = await _converter.ConvertAsync(_content, default);

        httpContext.Response.ContentType = ContentType;
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(csvString);

        await httpContext.Response.WriteAsync(csvString);
    }
}
