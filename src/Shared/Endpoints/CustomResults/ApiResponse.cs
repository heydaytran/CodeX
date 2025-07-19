namespace Endpoints.CustomResults;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public List<ErrorDetail>? Errors { get; init; }

    public ApiResponse(T data)
    {
        Success = true;
        Data = data;
        Errors = null;
    }

    public ApiResponse(List<ErrorDetail> errors)
    {
        Success = false;
        Errors = errors;
        Data = default;
    }
}

public class ErrorDetail
{
    public string Code { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
