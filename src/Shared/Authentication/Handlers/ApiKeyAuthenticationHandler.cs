using Microsoft.AspNetCore.Authentication;

namespace Authentication.Handlers;

//https://josef.codes/asp-net-core-protect-your-api-with-api-keys/
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private const string ProblemDetailsContentType = "application/problem+json";
    private readonly ISender _sender;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options, 
        ILoggerFactory logger, 
        System.Text.Encodings.Web.UrlEncoder encoder, 
        ISender sender,
#pragma warning disable CS0618 // Have to use ISystemClock until Microsoft.AspNetCore.Authentication is updated.
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
#pragma warning restore CS0618
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var httpContext = Request.HttpContext;
        if (!httpContext.Request.Headers.ContainsKey(ApiKeyConstants.HeaderName))
        {
            httpContext.Response.StatusCode = 401; 
            return AuthenticateResult.NoResult();
        }

        // Extract the API key and application name from headers
        var apiKey = httpContext.Request.Headers[ApiKeyConstants.HeaderName].ToString();

        if (string.IsNullOrEmpty(apiKey))
        {
            httpContext.Response.StatusCode = 401; // Bad Request
            return AuthenticateResult.NoResult();
        }

        // Validate the API key
        var validateApiKey = await _sender.Send(new GetApiKeyQuery(apiKey), Request.HttpContext.RequestAborted);
        
        if (!validateApiKey.IsError)
        {
            var key = validateApiKey.Value;
            var claims = new List<Claim> { new(ClaimTypes.Name, key.ApplicationName ?? "ApiKeyUser") };

            if (key.Roles != null)
            {
                claims.AddRange(key.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
            }

            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);
            
            return AuthenticateResult.Success(ticket);
        }
        
        return AuthenticateResult.NoResult();
        
    }
    
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = 401;
        Response.ContentType = ProblemDetailsContentType;
        var problemDetails = new UnauthorizedProblemDetails();

        await Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
    
    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = 403;
        Response.ContentType = ProblemDetailsContentType;
        var problemDetails = new ForbiddenProblemDetails();

        await Response.WriteAsync(JsonSerializer.Serialize(problemDetails, DefaultJsonSerializerOptions.Options));
    }

    private static class DefaultJsonSerializerOptions
    {
        public static JsonSerializerOptions Options =>
            new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
    }
}