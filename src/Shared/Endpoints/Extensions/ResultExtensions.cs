using Endpoints.CustomResults;

namespace Endpoints.Extensions;

public static class ResultExtensions
{
    private static readonly MediaTypeHeaderValue DefaultContentErrorMediaType = new(MediaTypeNames.Text.Plain)
    {
        Charset = Encoding.UTF8.WebName,
    };

    public static IResult Csv<T>(this IResultExtensions _, T content, ICsvConverter<T> converter) =>
        new CsvContentResult<T>(content, converter);

    public static IResult Gpx<T>(this IResultExtensions _, T content, IGpxConverter<T> converter) => 
        new GpxContentResult<T>(content, converter);

    public static IResult SevenZip(this IResultExtensions _, string content) =>
        new SevenZipContentResult(content);

    public static IResult Zip<T>(this IResultExtensions _, T content, IZipArchiveMaker<T> converter, bool enableRangeProcessing = false, bool surpressRangeNotSatisfiable = false) =>
        new ZipContentResult<T>(content, converter, enableRangeProcessing, surpressRangeNotSatisfiable);

    public static IResult RangedContent(this IResultExtensions _, string content, string contentType) => 
        new RangedContentResult(content, contentType);

    public static IResult Utf8TextHtml<T>(this IResultExtensions _, T content, IHtmlConverter<T> converter) =>
        Results.Extensions.Utf8TextHtmlContent(converter.Convert(content));

    public static IResult Utf8TextHtmlContent(this IResultExtensions _, string? content) => 
        Results.Content(
            content,
            new MediaTypeHeaderValue(MediaTypeNames.Text.Html)
            {
                Charset = Encoding.UTF8.WebName,
            });

    public static IResult Xml<T>(this IResultExtensions _, T content, IXmlConverter<T> converter, MediaTypeHeaderValue? mediaType = null, bool includeDeclaration = false, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new XmlContentResult<T>(content, converter, mediaType, includeDeclaration, statusCode);

    public static IResult ErrorWithSubStatus(this IResultExtensions _, int subStatusCode, string? subStatusHeaderName = null) 
        => new ErrorWithSubStatusResult(subStatusCode, subStatusHeaderName);

    public static IResult Error(this IResultExtensions _, IList<Error> errors)
    {
        if (!errors.Any())
        {
            return Results.Ok();
        }

        var firstError = errors.First();
        return firstError.Type switch
        {
            ErrorType.NotFound => Results.NotFound(),
            ErrorType.Validation => Results.StatusCode((int)HttpStatusCode.InternalServerError),
            _ => Results.StatusCode((int)HttpStatusCode.InternalServerError)
        };
    }

    public static IResult ContentError(this IResultExtensions _, IList<Error> errors, MediaTypeHeaderValue? mediaType = null)
    {
        if (!errors.Any())
        {
            return Results.Ok();
        }

        mediaType ??= DefaultContentErrorMediaType;

        var firstError = errors.First();
        var content = $"{firstError.Code}:{firstError.Description}";
        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => HttpStatusCode.NotFound,
            ErrorType.Validation => HttpStatusCode.InternalServerError,
            _ => HttpStatusCode.InternalServerError
        };

        return Results.Content(content, contentType: mediaType.ToString(), statusCode: (int)statusCode);
    }
}
