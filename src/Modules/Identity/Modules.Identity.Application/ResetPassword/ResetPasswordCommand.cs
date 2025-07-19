using Application.Messaging;
using MediatR;

namespace Modules.Identity.Application.ResetPassword;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : ICommand<Unit>;