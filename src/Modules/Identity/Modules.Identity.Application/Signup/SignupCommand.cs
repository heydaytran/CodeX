using Application.Messaging;
using MediatR;

namespace Modules.Identity.Application.Signup;

public record SignupCommand(string FullName, string Email, string Password) : ICommand<Unit>;