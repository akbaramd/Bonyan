namespace Bonyan.Messaging;

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
        IBonMessageBus messageBus)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        Headers = headers ?? new Dictionary<string, object>();
        ReplyTo = replyTo ?? throw new ArgumentNullException(nameof(replyTo));
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
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

        return _messageBus.SendAsync(
            serviceName: ReplyTo,
            message: response,
            headers: headers,
            correlationId: CorrelationId,
            isReply: true,
            cancellationToken
        );
    }

    /// <summary>
    /// Publishes a message to all subscribers.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="headers">Optional headers to include with the message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task PublishAsync<TMessage>(
        TMessage message,
        IDictionary<string, object>? headers = null,
        CancellationToken cancellationToken = default)
        where TMessage : class
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        return _messageBus.PublishAsync(
            message: message,
            headers: headers,
            correlationId: CorrelationId,
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Sends a message to a specific service without expecting a response.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message to send.</typeparam>
    /// <param name="serviceName">The name of the service to send the message to.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="headers">Optional headers to include with the message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SendAsync<TMessage>(
        string serviceName,
        TMessage message,
        IDictionary<string, object>? headers = null,
        CancellationToken cancellationToken = default)
        where TMessage : class
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        return _messageBus.SendAsync(
            serviceName: serviceName,
            message: message,
            headers: headers,
            correlationId: CorrelationId,
            isReply: false,
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Sends a message to a specific service and waits for a response.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message to send.</typeparam>
    /// <typeparam name="TResponse">The type of the expected response.</typeparam>
    /// <param name="serviceName">The name of the service to send the message to.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="headers">Optional headers to include with the message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The response from the service.</returns>
    public Task<TResponse> SendAsync<TMessage, TResponse>(
        string serviceName,
        TMessage message,
        IDictionary<string, object>? headers = null,
        CancellationToken cancellationToken = default)
        where TMessage : class
        where TResponse : class
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        return _messageBus.SendAsync<TMessage, TResponse>(
            serviceName: serviceName,
            message: message,
            headers: headers,
            correlationId: CorrelationId,
            isReply: false,
            cancellationToken: cancellationToken
        );
    }
}
