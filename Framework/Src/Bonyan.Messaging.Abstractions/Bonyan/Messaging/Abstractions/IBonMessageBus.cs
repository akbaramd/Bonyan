using Bonyan.Messaging;

public interface IBonMessageBus
{
    Task<TResponse> SendAsync<TMessage, TResponse>(
        string serviceName,
        TMessage message,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,   
        bool isReply = false,
        CancellationToken cancellationToken = default)
        where TMessage : class where TResponse : class;

    Task SendAsync<TMessage>(
        string serviceName,
        TMessage message,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,
        bool isReply = false,
        CancellationToken cancellationToken = default)
        where TMessage : class;

    Task PublishAsync<TMessage>(
        TMessage message,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class;

    /// <summary>
    /// Subscribes to a specific message type.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message to subscribe to.</typeparam>
    /// <param name="queueName">The queue name to bind the subscription to.</param>
    /// <param name="handler">The handler to process received messages.</param>
    void Subscribe<TMessage>(
        string queueName,
        Func<BonMessageContext<TMessage>, Task> handler)
        where TMessage : class;


}