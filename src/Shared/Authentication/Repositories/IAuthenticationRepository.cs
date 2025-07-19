namespace Authentication.Repositories;

public interface IAuthenticationRepository
{
    Task<ApiKey?> GetApiKey(string key, CancellationToken cancellationToken);
}