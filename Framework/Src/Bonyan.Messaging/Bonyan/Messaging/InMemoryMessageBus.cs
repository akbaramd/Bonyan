using Bonyan.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging;

public class InMemoryMessageBus : IBonMessageBus
{
    private readonly IEnumerable<IBonMessageProducer> _producers;

    public InMemoryMessageBus(IServiceProvider serviceProvider)
    {
        _producers = serviceProvider.GetServices<IBonMessageProducer>();
    }

    public async Task<TResponse> SendAsync<TMessage, TResponse>(
        string serviceName,
        TMessage message,
        IDictionary<string, string>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class
    {
        var producer = ResolveProducer(serviceName);
        if (producer == null)
        {
            throw new InvalidOperationException($"No producer found for service name '{serviceName}'");
        }

        return await producer.SendAsync<TMessage, TResponse>(serviceName, message, headers, correlationId, cancellationToken);
    }

    public async Task SendAsync<TMessage>(
        string serviceName,
        TMessage message,
        IDictionary<string, string>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class
    {
        var producer = ResolveProducer(serviceName);
        if (producer == null)
        {
            throw new InvalidOperationException($"No producer found for service name '{serviceName}'");
        }

        await producer.SendAsync(serviceName, message, headers, correlationId, cancellationToken);
    }

    public async Task PublishAsync<TMessage>(
        TMessage message,
        IDictionary<string, string>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class
    {
        foreach (var producer in _producers)
        {
            await producer.PublishAsync(message, headers, correlationId, cancellationToken);
        }
    }

    private IBonMessageProducer? ResolveProducer(string serviceName)
    {
        return _producers.FirstOrDefault(p => p.GetType().Name.Contains(serviceName, StringComparison.OrdinalIgnoreCase));
    }
}
