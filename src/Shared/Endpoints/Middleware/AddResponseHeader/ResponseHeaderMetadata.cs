namespace Endpoints.Middleware.AddResponseHeader;

public record ResponseHeaderMetadata(string Key, Func<HttpResponse, StringValues?> Value, Func<HttpContext, bool>? Condition = null)
{
    public StringValues? GetValue(HttpResponse response) => Value(response);
}