using Application.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Identity.Application.UserUseCase.UpdateUser
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.UserDTO.Id)
                .NotEmpty().WithMessage("User ID is required.")
                .NotNull().WithMessage("User ID cannot be null.");
            RuleFor(x => x.UserDTO.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.UserDTO.UserName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");
            RuleFor(x => x.UserDTO.PhoneNumber).Must(ValidatePhoneNumber.IsValidPhoneNumber).WithMessage("Invalid phone number.");

        }
    }
}
