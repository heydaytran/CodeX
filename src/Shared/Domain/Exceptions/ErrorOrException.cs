using Domain.Extensions;

namespace Domain.Exceptions;

public sealed class ErrorOrException(List<Error> errors) 
    : Exception(errors.ToLogString())
{
    public List<Error> Errors { get; } = errors;
}