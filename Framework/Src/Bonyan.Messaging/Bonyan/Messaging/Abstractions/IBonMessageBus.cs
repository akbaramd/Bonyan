namespace Bonyan.Messaging.Abstractions;

public interface IBonMessageBus 
{
    Task<TResponse> SendAsync<TMessage, TResponse>(
        string serviceName,
        TMessage message,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,   
        CancellationToken cancellationToken = default)
        where TMessage : class
        where TResponse : class;

    Task SendAsync<TMessage>(
        string serviceName,
        TMessage message,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class;
    Task PublishAsync<TMessage>(
        TMessage message,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class;
}