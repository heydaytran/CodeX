namespace Endpoints.CustomResults;

public interface IGpxConverter<T>
{
    XDocument Convert(T value);
}
