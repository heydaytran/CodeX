using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Routing;

namespace Endpoints.Extensions;

public static class ApiVersioningExtensions
{
    /// <summary>
    /// Creates a reusable API version set with the given versions.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="versions">List of API versions to register.</param>
    /// <returns>An ApiVersionSet instance.</returns>
    public static ApiVersionSet CreateApiVersionSet(this IEndpointRouteBuilder app, params int[] versions)
    {
        var versionSetBuilder = app.NewApiVersionSet();

        // Add each version dynamically
        foreach (var version in versions)
        {
            versionSetBuilder.HasApiVersion(new ApiVersion(version));
        }

        return versionSetBuilder.ReportApiVersions().Build();
    }
}