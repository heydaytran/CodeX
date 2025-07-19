using Application.Audit;
using Application.Messaging;
using Authentication.Entities;
using ErrorOr;
using Infrastructure.Audit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Modules.Identity.Application.Auth.Login;
using Modules.Identity.Application.Signup;
using Modules.Identity.Domain.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Identity.Application.Auth.FacebookLogin;

public class FacebookLoginHandler(UserManager<User> userManager,
        IAuditLogger auditLogger,
        ITokenService tokenService) : ICommandHandler<FacebookLoginCommand, LoginResponse>
{
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IAuditLogger _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
    private readonly ITokenService _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));


    public async Task<ErrorOr<LoginResponse>> Handle(FacebookLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                LockoutEnabled = true
            };

            var res = await _userManager.CreateAsync(user);
            if (!res.Succeeded)
            {
                var errors = string.Join(", ", res.Errors.Select(e => e.Description));
                await _auditLogger.LogAsync("LoginFacebook", request.Email, "Failure", errors);
                return Error.Validation("Identity", errors);
            }
            await _userManager.AddLoginAsync(user, new UserLoginInfo("Facebook", request.Email, "Facebook"));
        }

        if (await _userManager.IsLockedOutAsync(user!))
        {
            await _auditLogger.LogAsync("Login", request.Email, "Failure", "Account is locked");
            return Error.Unauthorized("Account is locked. Try again later.");
        }


        await _userManager.ResetAccessFailedCountAsync(user!);

        var accessToken = _tokenService.GenerateJwtToken(user!);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _userManager.SetAuthenticationTokenAsync(
            user!,
            loginProvider: "Facebook",
            tokenName: "RefreshToken",
            tokenValue: refreshToken);

        await _auditLogger.LogAsync("LoginFacebook", request.Email, "Success");

        return new LoginResponse(accessToken, refreshToken);
    }
}
