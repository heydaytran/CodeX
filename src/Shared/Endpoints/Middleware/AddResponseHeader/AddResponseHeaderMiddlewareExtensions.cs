namespace Endpoints.Middleware.AddResponseHeader;

public static class AddResponseHeaderMiddlewareExtensions
{
    private static readonly ResponseHeaderMetadata NoCacheNoStoreNoTransformCacheControlHeader = new("Cache-Control", _ => "no-cache, no-store, no-transform");
    private static readonly ResponseHeaderMetadata NeverExpiresHeader = new("Expires", _ => "-1");
    private static readonly ResponseHeaderMetadata PrivateCacheControlHeader = new("Cache-Control", _ => "private");

    public static IApplicationBuilder UseAddResponseHeader(this IApplicationBuilder builder)
        => builder.UseMiddleware<AddResponseHeaderMiddleware>();

    public static IApplicationBuilder UseAddResponseHeader(this IApplicationBuilder builder, AddResponseHeaderOptions options)
        => builder.UseMiddleware<AddResponseHeaderMiddleware>(Options.Create(options));

    public static TBuilder WithNoCacheAndNeverExpireResponseHeader<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder
            .WithNoCacheResponseHeader()
            .WithNeverExpiresHeader();
    }

    public static TBuilder WithNoCacheResponseHeader<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder
            .WithResponseHeader(NoCacheNoStoreNoTransformCacheControlHeader)
            .WithResponseHeader(NeverExpiresHeader);
    }

    public static TBuilder WithPrivateCacheControlHeader<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder => 
        builder.WithResponseHeader(PrivateCacheControlHeader);

    public static TBuilder WithNeverExpiresHeader<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder
            .WithResponseHeader(NeverExpiresHeader);
    }

    public static TBuilder WithResponseHeader<TBuilder>(this TBuilder builder, string key, StringValues value) where TBuilder : IEndpointConventionBuilder
    {
        ArgumentException.ThrowIfNullOrEmpty(value);

        return builder.WithResponseHeader(key, (HttpResponse response) => value);
    }

    public static TBuilder WithResponseHeader<TBuilder>(this TBuilder builder, string key, Func<HttpResponse, StringValues?> value) where TBuilder : IEndpointConventionBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(key);

        return builder.WithResponseHeader(new ResponseHeaderMetadata(key, value));
    }

    public static TBuilder WithResponseHeader<TBuilder>(this TBuilder builder, ResponseHeaderMetadata header) where TBuilder : IEndpointConventionBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(header);

        builder.Add(endpointBuilder =>
        {
            if (!endpointBuilder.Metadata.Contains(header))
            {
                endpointBuilder.Metadata.Add(header);
            }
        });

        return builder;
    }
}
