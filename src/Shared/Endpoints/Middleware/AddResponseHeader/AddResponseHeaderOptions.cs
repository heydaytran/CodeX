namespace Endpoints.Middleware.AddResponseHeader;

public class AddResponseHeaderOptions
{
    private readonly IList<ResponseHeaderMetadata> _baseHeaders = [];

    public IReadOnlyList<ResponseHeaderMetadata> BaseHeaders => _baseHeaders.AsReadOnly();

    public void Add(string key, StringValues value)
    {
        _baseHeaders.Add(new ResponseHeaderMetadata(key, _ => value));
    }
}