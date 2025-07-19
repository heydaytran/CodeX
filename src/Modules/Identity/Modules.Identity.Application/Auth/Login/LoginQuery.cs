using Application.Messaging;

namespace Modules.Identity.Application.Auth.Login;

public record LoginQuery(string Email, string Password) : IQuery<LoginResponse>;