using FluentValidation;
using Application.Common;

namespace Modules.Customer.Application.Customer.UpdateCustomer;

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.CustomerDTO.FirstName).NotEmpty().WithMessage("First name cannot be empty");
        RuleFor(x => x.CustomerDTO.LastName).NotEmpty().WithMessage("Last name cannot be empty");
        RuleFor(x => x.CustomerDTO.ContactInformation.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Invalid email format.");
        RuleFor(x => x.CustomerDTO.ContactInformation.PhoneNumber).NotEmpty().WithMessage("Phone number is required.").Must(ValidatePhoneNumber.IsValidPhoneNumber).WithMessage("Invalid phone number.");
    }
}