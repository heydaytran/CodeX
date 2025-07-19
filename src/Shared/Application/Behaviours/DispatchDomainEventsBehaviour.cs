using Domain.Primitives;

namespace Application.Behaviours;

public class DispatchDomainEventsBehaviour<TRequest, TResponse>(IPublisher publisher) : IPipelineBehavior<TRequest, TResponse> 
     where TRequest : notnull
     where TResponse : IErrorOr<IEntity>
{
    private readonly IPublisher _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();
        if (response.IsError)
        {
            return response;
        }

        var events = response.Value.GetDomainEvents();
        foreach (var e in events)
        {
            await _publisher.Publish(e, cancellationToken);
        }

        response.Value.ClearDomainEvents();

        return response;
    }
}