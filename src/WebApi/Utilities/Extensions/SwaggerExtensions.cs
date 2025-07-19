namespace WebApi.Utilities.Extensions;

internal static class SwaggerExtensions
{
    internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "LSG Nexus API",
                Version = "v1",
                Description = "LSG Nexus API."
            });

            // Define the Bearer token security scheme
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            // Define the x-api-key security scheme
            options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "API Key Authorization header using the x-api-key scheme. Example: \"x-api-key: {apiKey}\"",
                Name = ApiKeyAuthenticationOptions.DefaultScheme,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            // Add security requirements for both Bearer and x-api-key
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                },
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        },
                        Scheme = "apiKey",
                        Name = "x-api-key",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            // Other Swagger configuration
            options.CustomSchemaIds(t => t.ToString());

        });

        return services;
    }

    internal static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api.Template v1");
        });

        return app;
    }
}