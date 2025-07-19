namespace Authentication.GetApiKey;

public class GetApiKeyQueryHandler(IAuthenticationRepository authenticationRepository) : IQueryHandler<GetApiKeyQuery, ApiKey>
{
    public async Task<ErrorOr<ApiKey>> Handle(GetApiKeyQuery request, CancellationToken cancellationToken)
    {
        var res = await authenticationRepository.GetApiKey(request.ApiKey, cancellationToken);
        if (res == null)
        {
            return Error.NotFound("Invalid api key");
        }

        return res;
    }
}