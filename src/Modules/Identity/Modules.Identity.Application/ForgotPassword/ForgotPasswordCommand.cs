using Application.Messaging;
using MediatR;

namespace Modules.Identity.Application.ForgotPassword;

public record ForgotPasswordCommand(string Email) : ICommand<Unit>;