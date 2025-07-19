namespace Endpoints.Middleware.StatusCodeWhenHeaderExists;

public class StatusCodeWhenHeaderExistsMetadata
{
    public required HttpStatusCode StatusCode { get; set; }

    public required string HeaderName { get; set; }
}
