namespace Endpoints.Middleware.OffByOneRangeHeaderFixer;

public static class StatusCodeWhenHeaderExistsMetadataExtensions
{
    public static IApplicationBuilder UseOffByOneRangeHeaderFixer(this IApplicationBuilder builder)
        => builder.UseMiddleware<OffByOneRangeHeaderFixerMiddleware>();
}