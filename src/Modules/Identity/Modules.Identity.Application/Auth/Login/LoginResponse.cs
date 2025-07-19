namespace Modules.Identity.Application.Auth.Login;

public record LoginResponse(string Token, string RefreshToken);