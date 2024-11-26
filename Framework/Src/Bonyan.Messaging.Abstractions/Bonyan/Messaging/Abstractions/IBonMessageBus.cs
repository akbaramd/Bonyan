namespace Bonyan.Messaging.Abstractions;

/// <summary>
/// Interface for a centralized message bus that routes messages to producers.
/// </summary>
public interface IBonMessageBus
{
    Task<TResponse> SendAsync<TMessage, TResponse>(
        string serviceName,
        TMessage message,
        IDictionary<string, string>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class;

    Task SendAsync<TMessage>(
        string serviceName,
        TMessage message,
        IDictionary<string, string>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class;

    Task PublishAsync<TMessage>(
        TMessage message,
        IDictionary<string, string>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class;
}