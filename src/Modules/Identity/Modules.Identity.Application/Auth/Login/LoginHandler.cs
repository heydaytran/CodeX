using Application.Audit;
using Application.Messaging;
using Authentication.Entities;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Modules.Identity.Domain.Auth;

namespace Modules.Identity.Application.Auth.Login;

public class LoginHandler : IQueryHandler<LoginQuery, LoginResponse>
{
    private readonly ITokenService _tokenService;
    private readonly IAuditLogger _auditLogger;
    private readonly UserManager<User> _userManager;
    public LoginHandler(ITokenService tokenService, IAuditLogger auditLogger, UserManager<User> userManager)
    {
        _tokenService = tokenService;
        _auditLogger = auditLogger;
        _userManager = userManager;
    }

    public async Task<ErrorOr<LoginResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Email);
        if (user is null)
        {
            await _auditLogger.LogAsync("Login", request.Email, "Failure", "User not found");
            return Error.NotFound("User not found.");
        }


        if (await _userManager.IsLockedOutAsync(user))
        {
            await _auditLogger.LogAsync("Login", request.Email, "Failure", "Account is locked");
            return Error.Unauthorized("Account is locked. Try again later.");
        }
            
        
        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            await _userManager.AccessFailedAsync(user);
            await _auditLogger.LogAsync("Login", request.Email, "Failure", "Invalid password");
            return Error.Unauthorized("Invalid password.");
        }
        
        await _userManager.ResetAccessFailedCountAsync(user);
        
        var accessToken = _tokenService.GenerateJwtToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        await _userManager.SetAuthenticationTokenAsync(
            user,
            loginProvider: "Default",
            tokenName: "RefreshToken",
            tokenValue: refreshToken);

        await _auditLogger.LogAsync("Login", request.Email, "Success");
        
        return new LoginResponse(accessToken, refreshToken);
    }
}