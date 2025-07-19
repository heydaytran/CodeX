namespace Endpoints.CustomResults;

public interface ICsvConverter<T>
{
    Task<string> ConvertAsync(T value, CancellationToken cancellationToken);
}
