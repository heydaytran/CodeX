namespace Application.Messaging;

public interface IErrorResponse
{
    List<ErrorDto> Errors { get; set; }
}

public class ErrorResponse : IErrorResponse
{
    public List<ErrorDto> Errors { get; set; } = [];
}

public static class IErrorResponseExtensions
{
    public static IErrorResponse ToResponse(this List<Error> errors) =>
        new ErrorResponse { Errors = errors.Select(ErrorDto.From).ToList() };

    public static ErrorOr<TValue> ToErrorOr<TValue>(this IErrorResponse response) =>
        ErrorOr<TValue>.From(response.Errors.Select(e => e.ToError()).ToList());
}