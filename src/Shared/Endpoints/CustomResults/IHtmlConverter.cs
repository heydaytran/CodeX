namespace Endpoints.CustomResults;

public interface IHtmlConverter<T>
{
    string Convert(T value);
}
