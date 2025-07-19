using Domain.Extensions;

namespace Endpoints.CustomResults;

public sealed class XmlContentResult<T> : IResult
{
    private static readonly XDeclaration DefaultDeclaration = new("1.0", "utf-8", null);

    private readonly T _content;
    private readonly IXmlConverter<T> _converter;
    private readonly MediaTypeHeaderValue _mediaType;
    private readonly bool _includeDeclaration;

    private static readonly MediaTypeHeaderValue DefaultMediaType = new(MediaTypeNames.Text.Xml)
    {
        Charset = Encoding.UTF8.WebName,
    };

    public XmlContentResult(T content, IXmlConverter<T> converter, MediaTypeHeaderValue? mediaType = null, bool includeDeclaration = false, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        _content = content;
        _converter = converter;
        _mediaType = mediaType ?? DefaultMediaType;
        _includeDeclaration = includeDeclaration;
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }

    /// <inherit />
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var xmlElement = _converter.Convert(_content);
        var xmlDocument = new XDocument(DefaultDeclaration, xmlElement);
        var xmlString = xmlDocument.ToString(_includeDeclaration);

        httpContext.Response.ContentType = _mediaType.ToString();
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(xmlString);
        httpContext.Response.StatusCode = (int)StatusCode;

        await httpContext.Response.WriteAsync(xmlString);
    }
}