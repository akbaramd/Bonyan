namespace Bonyan.Messaging.Abstractions
{
    /// <summary>
    /// Generalized producer abstraction for sending and publishing messages to a broker.
    /// </summary>
    public interface IBonMessageProducer
    {

        /// <summary>
        /// Publishes a message to all subscribers.
        /// </summary>
        Task PublishAsync<TMessage>(
            string serviceName,
            TMessage message,
            IDictionary<string, object>? headers = null,
            string? correlationId = null,
            string? replyQueue = null, // Added parameter
            CancellationToken cancellationToken = default)
            where TMessage : class;
    }
}