namespace Bonyan.Mediators;

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
    Task<TResponse> SendAsync<TResponse>(
        IBonCommand<TResponse> command,
        CancellationToken cancellationToken = default);

    Task SendAsync(
        IBonCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a query to a single handler and expects a response.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the handler.</returns>
    Task<TResponse> QueryAsync<TResponse>(
        IBonQuery<TResponse> query,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Publishes an event to multiple subscribers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventMessage">The event to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync(
        IBonEvent eventMessage,
        CancellationToken cancellationToken = default);
}
