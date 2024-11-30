namespace Bonyan.Messaging.Abstractions;

/// <summary>
/// Interface for a producer that handles messaging to external systems.
/// </summary>
public interface IBonMessageProducer
{
    Task<TResponse> SendAsync<TMessage, TResponse>(
        string serviceName,
        TMessage message,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,   
        bool isReply = false,
        CancellationToken cancellationToken = default)
        where TMessage : class
        where TResponse : class;

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
}
