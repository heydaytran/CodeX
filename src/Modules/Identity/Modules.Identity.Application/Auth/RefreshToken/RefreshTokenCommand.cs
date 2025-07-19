using Application.Messaging;
using Modules.Identity.Application.Auth.Login;

namespace Modules.Identity.Application.Auth.RefreshToken;

public record RefreshTokenCommand(string UserId, string Token) : ICommand<LoginResponse>;