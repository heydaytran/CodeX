using Application.Messaging;
using Authentication.Entities;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Modules.Identity.Application.ForgotPassword;

public class ForgotPasswordHandler : ICommandHandler<ForgotPasswordCommand, Unit>
{
    private readonly UserManager<User> _userManager;

    public ForgotPasswordHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ErrorOr<Unit>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            return Unit.Value;
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        var resetUrl = $"https://yourfrontend.com/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(resetToken)}";

        // await _emailSender.SendAsync(
        //     to: user.Email,
        //     subject: "Reset your password",
        //     body: $"Click here to reset your password: {resetUrl}"
        // );

        return Unit.Value;
    }
}