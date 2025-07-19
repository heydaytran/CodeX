using Application.Messaging;
using Application.Token;
using Authentication.Database;
using Authentication.Entities;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Domain.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace Modules.Identity.Application.Auth.Logout;

public class LogoutHandler : ICommandHandler<LogoutCommand, Unit>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenRevocationStore _tokenRevocation;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthenticationDbContext _identityDbContext;
    private readonly ITokenService _tokenService;
    
    public LogoutHandler(UserManager<User> userManager, 
        ITokenRevocationStore tokenRevocation, 
        IHttpContextAccessor httpContextAccessor,
        AuthenticationDbContext authenticationDbContext,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenRevocation = tokenRevocation;
        _httpContextAccessor = httpContextAccessor;
        _identityDbContext = authenticationDbContext ;
        _tokenService = tokenService;
    }
    
    public async Task<ErrorOr<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) return Error.NotFound("User not found.");

        // get provider name from user logins
        var userToken = await _tokenService.GetUserTokenAsync(user.Id, cancellationToken);
        var providerName = userToken?.LoginProvider ?? "Default";


        // Revoke the refresh token stored in AspNetUserTokens
        await _userManager.RemoveAuthenticationTokenAsync(user, providerName, "RefreshToken");

        var jti = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        if (!string.IsNullOrEmpty(jti))
        {
            await _tokenRevocation.RevokeAsync(jti, TimeSpan.FromMinutes(15));
        }
        
        return Unit.Value;
    }
}