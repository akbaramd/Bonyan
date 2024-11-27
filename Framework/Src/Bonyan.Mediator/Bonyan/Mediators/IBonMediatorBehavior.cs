namespace Bonyan.Mediators;

public interface IBonMediatorBehavior<TRequest, TResponse> 
{
 Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken);
}

public interface IBonMediatorBehavior<TRequest>
{
    Task HandleAsync(TRequest request, Func<Task> next, CancellationToken cancellationToken);
}