using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.Abstractions.Mediators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.Messaging.Mediators;

/// <summary>
/// In-memory implementation of IBonMediator using handlers for commands, queries, and events.
/// </summary>
public class InMemoryBonMediator : IBonMediator
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryBonMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Sends a command with a response to its handler.
    /// </summary>
    public async Task<TResponse> SendAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : IBonCommand<TResponse>
    {
        var handler = _serviceProvider.GetService<IBonCommandHandler<TCommand, TResponse>>();
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler found for command type {typeof(TCommand).Name}");
        }

        return await handler.HandleAsync(command, cancellationToken);
    }

    /// <summary>
    /// Sends a command without a response to its handler.
    /// </summary>
    public async Task SendAsync<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : IBonCommand
    {
        var handler = _serviceProvider.GetService<IBonCommandHandler<TCommand>>();
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler found for command type {typeof(TCommand).Name}");
        }

        await handler.HandleAsync(command, cancellationToken);
    }

    /// <summary>
    /// Sends a query to its handler and expects a response.
    /// </summary>
    public async Task<TResponse> QueryAsync<TQuery, TResponse>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IBonQuery<TResponse>
    {
        var handler = _serviceProvider.GetService<IBonQueryHandler<TQuery, TResponse>>();
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler found for query type {typeof(TQuery).Name}");
        }

        return await handler.HandleAsync(query, cancellationToken);
    }

    /// <summary>
    /// Publishes an event to multiple subscribers.
    /// </summary>
    public async Task PublishAsync<TEvent>(
        TEvent eventMessage,
        CancellationToken cancellationToken = default)
        where TEvent : IBonEvent
    {
        var handlers = _serviceProvider.GetServices<IBonEventHandler<TEvent>>();
        if (!handlers.Any())
        {
            // If no handlers are found, log or silently ignore
            Console.WriteLine($"No event handlers found for event type {typeof(TEvent).Name}");
            return;
        }

        var tasks = handlers.Select(handler => handler.HandleAsync(eventMessage, cancellationToken));
        await Task.WhenAll(tasks);
    }
}
