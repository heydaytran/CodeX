namespace Endpoints.CustomResults;

public interface IXmlConverter<T>
{
    XElement Convert(T content);
}