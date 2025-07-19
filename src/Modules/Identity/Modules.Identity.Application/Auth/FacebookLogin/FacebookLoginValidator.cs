using FluentValidation;
using Modules.Identity.Application.Auth.FacebookLogin;

namespace Modules.Identity.Application.Auth;

public class FacebookLoginValidator : AbstractValidator<FacebookLoginCommand>
{
    public FacebookLoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name is too long.");
    }
}