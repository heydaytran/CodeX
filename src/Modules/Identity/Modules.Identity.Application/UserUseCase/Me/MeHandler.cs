using System.Security.Claims;
using Application.Messaging;
using Authentication.Database;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Modules.Identity.Application.UserUseCase.Me;

public class MeHandler : IQueryHandler<MeQuery, MeResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthenticationDbContext _dbContext;

    public MeHandler(IHttpContextAccessor httpContextAccessor, AuthenticationDbContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<ErrorOr<MeResponse>> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true)
        {
            return Error.Unauthorized(description: "User is not authenticated.");
        }

        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Error.Unauthorized(description: "User ID claim not found.");
        }

        var dbUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (dbUser == null)
        {
            return Error.NotFound(description: $"User with ID '{userId}' not found.");
        }

        return new MeResponse(dbUser);

    }
}