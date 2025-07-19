using Application.Messaging;
using MediatR;

namespace Modules.Identity.Application.Auth.Logout;

public record LogoutCommand(string UserId) : ICommand<Unit>;