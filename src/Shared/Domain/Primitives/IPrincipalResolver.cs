namespace Domain.Primitives;

public interface IPrincipalResolver
{
    public Task<ErrorOr<IPrincipal>> ResolveAsync(CancellationToken cancellationToken = default);
}
