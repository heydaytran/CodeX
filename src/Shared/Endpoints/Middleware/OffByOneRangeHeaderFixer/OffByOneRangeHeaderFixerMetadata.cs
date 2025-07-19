using Endpoints.Extensions;

namespace Endpoints.Middleware.OffByOneRangeHeaderFixer;

public record OffByOneRangeHeaderFixerMetadata(Func<HttpRequest, bool> Enabled)
{
    public static readonly OffByOneRangeHeaderFixerMetadata AlwaysOn = new(request => true);

    public static readonly OffByOneRangeHeaderFixerMetadata HasCustomTicketerRangeHeader = new(request => request.IsTicketerRangeRequestEnabled());
}