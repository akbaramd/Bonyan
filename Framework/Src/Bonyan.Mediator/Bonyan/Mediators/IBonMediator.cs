namespace Bonyan.Messaging.Abstractions.Mediators;

/// <summary>
/// Represents a mediator bus for handling commands, queries, and events.
/// </summary>
public interface IBonMediator
{
    /// <summary>
    /// Sends a command to a single handler.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the handler.</returns>
    Task<TResponse> SendAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : IBonCommand<TResponse>;
    
    Task SendAsync<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : IBonCommand;
    /// <summary>
    /// Sends a query to a single handler and expects a response.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the handler.</returns>
    Task<TResponse> QueryAsync<TQuery, TResponse>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IBonQuery<TResponse>;
    
    /// <summary>
    /// Publishes an event to multiple subscribers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventMessage">The event to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync<TEvent>(
        TEvent eventMessage,
        CancellationToken cancellationToken = default)
        where TEvent : IBonEvent;
}
