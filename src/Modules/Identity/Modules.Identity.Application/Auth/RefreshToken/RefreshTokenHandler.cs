using Application.Audit;
using Application.Messaging;
using Authentication.Database;
using Authentication.Entities;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Auth.Login;
using Modules.Identity.Domain.Auth;

namespace Modules.Identity.Application.Auth.RefreshToken;

public class RefreshTokenHandler : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly ITokenService _tokenService;
    private readonly IAuditLogger _auditLogger;
    private readonly UserManager<User> _userManager;
    private readonly AuthenticationDbContext _identityDbContext;
    public RefreshTokenHandler(AuthenticationDbContext dbContext,
        ITokenService tokenService,
        IAuditLogger auditLogger,
        UserManager<User> userManager)
    {
        _tokenService = tokenService;
        _auditLogger = auditLogger;
        _userManager = userManager;
        _identityDbContext = dbContext;
    }

    public async Task<ErrorOr<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId; // GUID or string passed in the request
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            await _auditLogger.LogAsync("RefreshToken", request.Token, "Failure", "User Not Found");
            return Error.NotFound("User not found.");
        }

        // get provider name from user logins
        var userToken = await _tokenService.GetUserTokenAsync(user.Id, cancellationToken);
        var providerName = userToken?.LoginProvider ?? "Default";

        var storedToken = await _userManager.GetAuthenticationTokenAsync(user, providerName!.ToString(), "RefreshToken");

        if (storedToken != request.Token)
        {
            await _auditLogger.LogAsync("RefreshToken", request.Token, "Failure", "Invalid or expired refresh token");
            return Error.Unauthorized("Invalid refresh token.");
        }

        // Rotate refresh token
        await _userManager.RemoveAuthenticationTokenAsync(user, providerName, "RefreshToken");

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        await _userManager.SetAuthenticationTokenAsync(user, providerName, "RefreshToken", newRefreshToken);

        var newAccessToken = _tokenService.GenerateJwtToken(user);

        await _auditLogger.LogAsync("RefreshToken", user.Email!, "Success");

        return new LoginResponse(newAccessToken, newRefreshToken);

    }
}