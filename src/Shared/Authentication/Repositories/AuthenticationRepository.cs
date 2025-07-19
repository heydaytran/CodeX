using Application.Lifetimes;

namespace Authentication.Repositories;

public class AuthenticationRepository(AuthenticationDbContext context) : IAuthenticationRepository, IScoped
{
    private readonly AuthenticationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    
    public Task<List<ApiKey>> GetApiKeys(CancellationToken cancellationToken)
    {
        return _context.ApiKeys.ToListAsync(cancellationToken);
    }

    public Task<ApiKey?> GetApiKey(string key, CancellationToken cancellationToken)
    {
        return _context.ApiKeys.FirstOrDefaultAsync(x => x.ApiKeyHash == key && x.IsActive, cancellationToken);
    }
}