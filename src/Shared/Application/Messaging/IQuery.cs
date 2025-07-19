namespace Application.Messaging;

public interface IQuery<TResponse> : IRequest<ErrorOr<TResponse>>
{
}