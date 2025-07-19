using Application.Messaging;
using Authentication.Entities;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Modules.Identity.Application.Signup;

public class SignupHandler : ICommandHandler<SignupCommand, Unit>
{
    private readonly UserManager<User> _userManager;

    public SignupHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<ErrorOr<Unit>> Handle(SignupCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
        {
            return Error.Conflict("Email is already registered.");
        }

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            LockoutEnabled = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Error.Validation("Identity", errors);
        }
        // add user login info
        await _userManager.AddLoginAsync(user, new UserLoginInfo("Default", request.Email, "Default"));


        return Unit.Value;
    }
}