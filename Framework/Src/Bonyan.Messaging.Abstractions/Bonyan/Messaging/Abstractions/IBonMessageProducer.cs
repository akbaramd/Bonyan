namespace Bonyan.Messaging.Abstractions;

/// <summary>
/// Interface for a producer that handles messaging to external systems.
/// </summary>
public interface IBonMessageProducer
{
    /// <summary>
    /// Sends a message to a specific target service and expects a response.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="serviceName">The target service name.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="headers">Optional headers for the message.</param>
    /// <param name="correlationId">Optional correlation ID for tracing the message flow.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<TResponse> SendAsync<TMessage, TResponse>(
        string serviceName,
        TMessage message,
        IDictionary<string, string>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class;

    /// <summary>
    /// Sends a fire-and-forget message to a specific target service.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="serviceName">The target service name.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="headers">Optional headers for the message.</param>
    /// <param name="correlationId">Optional correlation ID for tracing the message flow.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SendAsync<TMessage>(
        string serviceName,
        TMessage message,
        IDictionary<string, string>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class;

    /// <summary>
    /// Publishes a message to multiple subscribers.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="headers">Optional headers for the message.</param>
    /// <param name="correlationId">Optional correlation ID for tracing the message flow.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task PublishAsync<TMessage>(
        TMessage message,
        IDictionary<string, string>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class;
}
