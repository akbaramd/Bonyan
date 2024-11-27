using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Mediators;

public class InMemoryBonMediator : IBonMediator
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryBonMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    // Handle commands with a response
    public async Task<TResponse> SendAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : IBonCommand<TResponse>
    {
        var behaviors = _serviceProvider.GetServices<IBonMediatorBehavior<TCommand, TResponse>>().ToList();
        var handler = _serviceProvider.GetService<IBonCommandHandler<TCommand, TResponse>>();
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler found for command type {typeof(TCommand).Name}");
        }

        async Task<TResponse> HandlerDelegate() => await handler.HandleAsync(command, cancellationToken);

        var pipeline = behaviors.Reverse<IBonMediatorBehavior<TCommand, TResponse>>()
            .Aggregate((Func<Task<TResponse>>)HandlerDelegate,
                (next, behavior) => () => behavior.HandleAsync(command, next, cancellationToken));

        return await pipeline();
    }

    // Handle commands without a response
    public async Task SendAsync<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : IBonCommand
    {
        var behaviors = _serviceProvider.GetServices<IBonMediatorBehavior<TCommand>>().ToList();
        var handler = _serviceProvider.GetService<IBonCommandHandler<TCommand>>();
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler found for command type {typeof(TCommand).Name}");
        }

        async Task HandlerDelegate() => await handler.HandleAsync(command, cancellationToken);

        var pipeline = behaviors.Reverse<IBonMediatorBehavior<TCommand>>()
            .Aggregate((Func<Task>)HandlerDelegate,
                (next, behavior) => () => behavior.HandleAsync(command, next, cancellationToken));

        await pipeline();
    }

    // Handle queries
    public async Task<TResponse> QueryAsync<TQuery, TResponse>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IBonQuery<TResponse>
    {
        var behaviors = _serviceProvider.GetServices<IBonMediatorBehavior<TQuery, TResponse>>().ToList();
        var handler = _serviceProvider.GetService<IBonQueryHandler<TQuery, TResponse>>();
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler found for query type {typeof(TQuery).Name}");
        }

        async Task<TResponse> HandlerDelegate() => await handler.HandleAsync(query, cancellationToken);

        var pipeline = behaviors.Reverse<IBonMediatorBehavior<TQuery, TResponse>>()
            .Aggregate((Func<Task<TResponse>>)HandlerDelegate,
                (next, behavior) => () => behavior.HandleAsync(query, next, cancellationToken));

        return await pipeline();
    }

    // Handle events
    public async Task PublishAsync<TEvent>(
        TEvent eventMessage,
        CancellationToken cancellationToken = default)
        where TEvent : IBonEvent
    {
        var behaviors = _serviceProvider.GetServices<IBonMediatorBehavior<TEvent>>().ToList();
        var handlers = _serviceProvider.GetServices<IBonEventHandler<TEvent>>().ToList();
        if (!handlers.Any())
        {
            Console.WriteLine($"No event handlers found for event type {typeof(TEvent).Name}");
            return;
        }

        // Pipeline for publishing events
        async Task HandlerDelegate()
        {
            var tasks = handlers.Select(handler => handler.HandleAsync(eventMessage, cancellationToken));
            await Task.WhenAll(tasks);
        }

        var pipeline = behaviors.Reverse<IBonMediatorBehavior<TEvent>>()
            .Aggregate((Func<Task>)HandlerDelegate,
                (next, behavior) => () => behavior.HandleAsync(eventMessage, next, cancellationToken));

        await pipeline();
    }
}