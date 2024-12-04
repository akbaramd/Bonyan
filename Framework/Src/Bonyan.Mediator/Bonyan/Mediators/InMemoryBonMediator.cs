using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bonyan.Mediators;

public class InMemoryBonMediator : IBonMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InMemoryBonMediator> _logger;

    public InMemoryBonMediator(IServiceProvider serviceProvider, ILogger<InMemoryBonMediator> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Handle commands with a response
    public async Task<TResponse> SendAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : IBonCommand<TResponse>
    {
        _logger.LogInformation("Handling command of type {CommandType}", command.GetType().Name);

        var behaviors = _serviceProvider.GetServices<IBonMediatorBehavior<TCommand, TResponse>>().ToList();
        var handler = _serviceProvider.GetService<IBonCommandHandler<TCommand, TResponse>>();
        if (handler == null)
        {
            _logger.LogError("No handler found for command type {CommandType}", command.GetType().Name);
            throw new InvalidOperationException($"No handler found for command type {command.GetType().Name}");
        }

        async Task<TResponse> HandlerDelegate() => await handler.HandleAsync(command, cancellationToken);

        var pipeline = behaviors.Reverse<IBonMediatorBehavior<TCommand, TResponse>>()
            .Aggregate((Func<Task<TResponse>>)HandlerDelegate,
                (next, behavior) => () => behavior.HandleAsync(command, next, cancellationToken));

        var response = await pipeline();
        _logger.LogInformation("Successfully handled command of type {CommandType}", command.GetType().Name);
        return response;
    }

    // Handle commands without a response
    public async Task SendAsync<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : IBonCommand
    {
        _logger.LogInformation("Handling command of type {CommandType}", command.GetType().Name);

        var behaviors = _serviceProvider.GetServices<IBonMediatorBehavior<TCommand>>().ToList();
        var handler = _serviceProvider.GetService<IBonCommandHandler<TCommand>>();
        if (handler == null)
        {
            _logger.LogError("No handler found for command type {CommandType}", command.GetType().Name);
            throw new InvalidOperationException($"No handler found for command type {command.GetType().Name}");
        }

        async Task HandlerDelegate() => await handler.HandleAsync(command, cancellationToken);

        var pipeline = behaviors.Reverse<IBonMediatorBehavior<TCommand>>()
            .Aggregate((Func<Task>)HandlerDelegate,
                (next, behavior) => () => behavior.HandleAsync(command, next, cancellationToken));

        await pipeline();
        _logger.LogInformation("Successfully handled command of type {CommandType}", command.GetType().Name);
    }

    // Handle queries
    public async Task<TResponse> QueryAsync<TQuery, TResponse>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IBonQuery<TResponse>
    {
        _logger.LogInformation("Handling query of type {QueryType}", query.GetType().Name);

        var behaviors = _serviceProvider.GetServices<IBonMediatorBehavior<TQuery, TResponse>>().ToList();
        var handler = _serviceProvider.GetService<IBonQueryHandler<TQuery, TResponse>>();
        if (handler == null)
        {
            _logger.LogError("No handler found for query type {QueryType}", query.GetType().Name);
            throw new InvalidOperationException($"No handler found for query type {query.GetType().Name}");
        }

        async Task<TResponse> HandlerDelegate() => await handler.HandleAsync(query, cancellationToken);

        var pipeline = behaviors.Reverse<IBonMediatorBehavior<TQuery, TResponse>>()
            .Aggregate((Func<Task<TResponse>>)HandlerDelegate,
                (next, behavior) => () => behavior.HandleAsync(query, next, cancellationToken));

        var response = await pipeline();
        _logger.LogInformation("Successfully handled query of type {QueryType}", query.GetType().Name);
        return response;
    }

    // Handle events
    public async Task PublishAsync<TEvent>(
        TEvent eventMessage,
        CancellationToken cancellationToken = default)
        where TEvent : IBonEvent
    {
        _logger.LogInformation("Publishing event of type {EventType}", eventMessage.GetType().Name);

        var behaviors = _serviceProvider.GetServices<IBonMediatorBehavior<TEvent>>().ToList();
        var handlers = _serviceProvider.GetServices<IBonEventHandler<TEvent>>().ToList();
        if (!handlers.Any())
        {
            _logger.LogWarning("No event handlers found for event type {EventType}", eventMessage.GetType().Name);
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
        _logger.LogInformation("Successfully published event of type {EventType}", eventMessage.GetType().Name);
    }
}
