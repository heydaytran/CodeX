using System.Text.Json.Serialization;
using Authentication.Database;
using Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using WebApi.Utilities.Extensions;
using WebApi.Utilities.Logging;
using WebApi.Utilities.Logging.Extensions;

LoggingUtility.Run(() =>
{
    var builder = WebApplication.CreateBuilder(args);

    var env = builder.Environment;

    // Logging.
    builder.Host.UseSerilogWithConfiguration();
    builder.Host.UseElasticEcho(builder.Configuration);
    // Register shared services.
    builder.Services
        .InstallServicesFromAssemblies(
            builder.Configuration,
            Endpoints.AssemblyReference.Assembly,
            Persistence.AssemblyReference.Assembly,
            Infrastructure.AssemblyReference.Assembly,
            Authentication.AssemblyReference.Assembly,
            AssemblyReference.Assembly);

    // Register modules.
    builder.Services
        .InstallModulesFromAssemblies(
            builder.Configuration,
            Modules.Identity.Infrastructure.AssemblyReference.Assembly,
            Modules.Configuration.Infrastructure.AssemblyReference.Assembly,
            Modules.Customer.Infrastructure.AssemblyReference.Assembly,
            Modules.Tenant.Infrastructure.AssemblyReference.Assembly
        );
    
    builder.Services.AddSingleton(TimeProvider.System);
    builder.Services.AddCompression();
    builder.Services.UseRateLimiter();
    builder.Services.UseApiVersioning();
    builder.Services.AddSwaggerDocumentation();
    builder.Services.AddDataProtection(builder.Configuration);
    builder.Services.AddResponseCaching();
    builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
    }).AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    
    builder.Services.AddIdentity<User, IdentityRole>(options =>
        {
            // Optional: lockout, password, etc.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<AuthenticationDbContext>()
        .AddDefaultTokenProviders();
    // builder.Services.AddIdentity<User, IdentityRole>()
    //     .AddEntityFrameworkStores<AuthenticationDbContext>()
    //     .AddDefaultTokenProviders();
    builder.Services.AddEndpointsApiExplorer();

    var app = builder.Build();
    app.UseCorrelationId();

    string envName = env.EnvironmentName;
    app.Logger.LogInformation("Running as environment {envName}.", envName);

    var responseWriter = UIResponseWriter.WriteHealthCheckUIResponseNoExceptionDetails;

    if (app.Environment.IsDevelopment())
    {
        responseWriter = UIResponseWriter.WriteHealthCheckUIResponse;

        app.UseSwaggerDocumentation();
        app.UseReDoc(c =>
        {
            c.DocumentTitle = "Lsg Nexus Api";
            c.SpecUrl = "/swagger/v1/swagger.json";
        });
    }

    app.UseCookiePolicy()
    .UseAuthentication()
    .UseAuthorization();


    // Health Checks
    app.MapControllers(); // Ensure controllers are used instead of MapCarter
    app.MapHealthChecks("/hc/liveness",
        new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = responseWriter,
        }).WithMetadata(new AllowAnonymousAttribute());

    app.MapHealthChecks("/hc/readiness",
        new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = responseWriter,
        }).WithMetadata(new AllowAnonymousAttribute());



    app
        .UseServiceUnavailable()
        .UseOffByOneRangeHeaderFixer()
        .UseAddResponseHeader()
        .UseHttpsRedirection()
        .UseMonitorResponse()
        .UseStatusCodeWhenHeaderExists()
        .UseRateLimiter()
        .UseResponseCaching()
        .UseResponseCompression()
        .UseOutputCache()
        .UseForwardedHeaders()
        .UseSerilogRequestLogging(o =>
        {
            o.IncludeQueryInRequestPath = true;
        });
    
    app.MapGet("/", async (context) =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(LandingPageHtml.GetContent());

    }).AllowAnonymous();

    app.Run();
});
