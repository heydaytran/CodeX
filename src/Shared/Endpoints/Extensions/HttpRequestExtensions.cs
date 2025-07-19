namespace Endpoints.Extensions;

public static class HttpRequestExtensions
{
    private const string TicketerRangeVersion1 = "1";
    private const string TicketerRangeVersion2 = "2";
    private static readonly IList<string> AllowedTicketerRangeVersions = [TicketerRangeVersion1, TicketerRangeVersion2];

    public static bool IsTicketerRangeRequestV1(this HttpRequest request) =>
        request.Headers.TryGetValue("X-T-R", out StringValues values) &&
        values.ToString().Equals(TicketerRangeVersion1, StringComparison.OrdinalIgnoreCase);

    public static bool IsTicketerRangeRequestEnabled(this HttpRequest request) => 
        request.Headers.TryGetValue("X-T-R", out StringValues values) && 
        AllowedTicketerRangeVersions.Any(v => values.ToString().Equals(v, StringComparison.OrdinalIgnoreCase));

    public static string? QueryOrForm(this HttpRequest? request, string paramName)
    {
        ArgumentException.ThrowIfNullOrEmpty(paramName, nameof(paramName));

        if (request is null)
        {
            return null;
        }

        // Check query string first.
        if (request.Query.TryGetValue(paramName, out var queryValues))
        {
            return queryValues;
        }

        // Then form.
        if (request.AllowsFormData() &&
            request.Form.TryGetValue(paramName, out var formValues))
        {
            return formValues;
        }

        return null;
    }

    public static bool AllowsFormData(this HttpRequest request)
    {
        var value = 
            request is not null &&
            request.Method.Equals(HttpMethod.Post.ToString(), StringComparison.OrdinalIgnoreCase) &&
            request.HasFormContentType();

        if (!value)
        {
            return false;
        }

        try
        {
            request?.Form.TryGetValue("trygetanykey", out _);

            return true;
        }
        catch (Exception)
        {
            // Safe to ignore, may have incorrect content disposition for example.
            return false;
        }
    }

    public static bool HasFormContentType(this HttpRequest request)
    {
        if (request is null)
        {
            return false;
        }

        var contentType = request.ContentTypeMediaType();

        return
            HasApplicationFormContentType(contentType) ||
            HasMultipartFormContentType(contentType);
    }

    private static MediaTypeHeaderValue? ContentTypeMediaType(this HttpRequest request)
    {
        if (request is null)
        {
            return null;
        }

        _ = MediaTypeHeaderValue.TryParse(request.ContentType, out MediaTypeHeaderValue? mt);

        return mt;
    }

    private static bool HasApplicationFormContentType(MediaTypeHeaderValue? contentType) =>
        // Content-Type: application/x-www-form-urlencoded; charset=utf-8
        contentType is not null &&
        contentType.MediaType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase);

    private static bool HasMultipartFormContentType(MediaTypeHeaderValue? contentType) =>
        // Content-Type: multipart/form-data; boundary=----WebKitFormBoundarymx2fSWqWSd0OxQqq
        contentType is not null &&
        contentType.MediaType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase);
}