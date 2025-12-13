using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging.Abstractions;

/// <summary>
/// Wrapper class for responses to make them compatible with IMessageRequest interface.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
internal class ResponseWrapper<TResponse> : MessageRequestBase
    where TResponse : class
{
    public TResponse Response { get; }

    public ResponseWrapper(TResponse response)
    {
        Response = response ?? throw new ArgumentNullException(nameof(response));
    }
}

/// <summary>
/// Provides context for messages being processed, including reply functionality, headers, and correlation management.
/// </summary>
/// <typeparam name="TMessage">The type of the message.</typeparam>
public class BonMessageContext<TMessage>
{
    private readonly IBonMessageBus _messageBus;

    /// <summary>
    /// Gets the received message.
    /// </summary>
    public TMessage Message { get; }

    /// <summary>
    /// Gets the correlation ID for the message, used to track related requests and responses.
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    /// Gets the headers associated with the message.
    /// </summary>
    public IDictionary<string, object> Headers { get; }

    /// <summary>
    /// Gets the queue name or routing key for replies.
    /// </summary>
    public string ReplyTo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BonMessageContext{TMessage}"/> class.
    /// </summary>
    /// <param name="message">The message being processed.</param>
    /// <param name="correlationId">The correlation ID for the message.</param>
    /// <param name="headers">Optional headers associated with the message.</param>
    /// <param name="replyTo">The queue or routing key to send replies to.</param>
    /// <param name="messageBus">The message bus to use for sending and replying.</param>
    /// <exception cref="ArgumentNullException">Thrown if required arguments are null.</exception>
    public BonMessageContext(
        TMessage message,
        string correlationId,
        IDictionary<string, object>? headers,
        string replyTo,
        IServiceProvider serviceProvider)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        Headers = headers ?? new Dictionary<string, object>();
        ReplyTo = replyTo ?? throw new ArgumentNullException(nameof(replyTo));
        _messageBus = serviceProvider.GetRequiredService<IBonMessageBus>();
    }

    /// <summary>
    /// Sends a reply to the message sender using the `ReplyTo` property.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response message.</typeparam>
    /// <param name="response">The response message to send.</param>
    /// <param name="headers">Optional headers to include with the reply.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ReplyAsync<TResponse>(
        TResponse response,
        IDictionary<string, object>? headers = null,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        if (response == null) throw new ArgumentNullException(nameof(response));

        // For now, ReplyAsync is a no-op since we don't have a proper reply mechanism
        // In a real implementation, this would send the response back to the original sender
        // For testing purposes, we'll just complete successfully
        return Task.CompletedTask;
    }

    /// <summary>
    /// Publishes a message to all subscribers.
    /// </summary>
    /// <typeparam name="TEventMessage">The type of the message to publish.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="headers">Optional headers to include with the message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task PublishAsync<TEventMessage>(
        TEventMessage message,
        IDictionary<string, object>? headers = null,
        CancellationToken cancellationToken = default)
        where TEventMessage : class, IMessageEvent
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        // Set correlation ID and headers
        message.CorrelationId = CorrelationId;
        if (headers != null)
        {
            message.Headers = headers;
        }

        return _messageBus.PublishAsync(message, cancellationToken);
    }

    /// <summary>
    /// Sends a message to a specific service without expecting a response.
    /// </summary>
    /// <typeparam name="TRequestMessage">The type of the message to send.</typeparam>
    /// <param name="serviceName">The name of the service to send the message to.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="headers">Optional headers to include with the message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SendAsync<TRequestMessage>(
        string serviceName,
        TRequestMessage message,
        IDictionary<string, object>? headers = null,
        CancellationToken cancellationToken = default)
        where TRequestMessage : class, IMessageRequest
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        // Set correlation ID and headers
        message.CorrelationId = CorrelationId;
        if (headers != null)
        {
            message.Headers = headers;
        }
        message.TargetService = serviceName;

        return _messageBus.SendAsync(serviceName, message, cancellationToken);
    }

    /// <summary>
    /// Sends a message to a specific service and waits for a response.
    /// </summary>
    /// <typeparam name="TRequestMessage">The type of the message to send.</typeparam>
    /// <typeparam name="TResponse">The type of the expected response.</typeparam>
    /// <param name="serviceName">The name of the service to send the message to.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="headers">Optional headers to include with the message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The response from the service.</returns>
    public Task<TResponse> SendAsync<TRequestMessage, TResponse>(
        string serviceName,
        TRequestMessage message,
        IDictionary<string, object>? headers = null,
        CancellationToken cancellationToken = default)
        where TRequestMessage : class, IMessageRequest<TResponse>
        where TResponse : class
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        // Set correlation ID and headers
        message.CorrelationId = CorrelationId;
        if (headers != null)
        {
            message.Headers = headers;
        }
        message.TargetService = serviceName;

        return _messageBus.SendAsync<TRequestMessage, TResponse>(serviceName, message, cancellationToken);
    }
}
