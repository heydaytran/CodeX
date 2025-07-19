namespace Application.Messaging;

public interface ICommandBase { }

public interface ICommand : IRequest, ICommandBase
{
}

public interface ICommand<TResponse> : IRequest<ErrorOr<TResponse>>, ICommandBase
{
}