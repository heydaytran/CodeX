namespace Endpoints.Middleware.AddResponseHeader;

internal sealed class AddResponseHeaderOptionsSetup : IConfigureOptions<AddResponseHeaderOptions>
{
    /// <inheritdoc />
    public void Configure(AddResponseHeaderOptions options) => options.Add("CV", "2");
}
