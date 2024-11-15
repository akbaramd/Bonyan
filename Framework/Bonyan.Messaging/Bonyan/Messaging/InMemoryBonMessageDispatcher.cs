using Bonyan.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging;

public class InMemoryBonMessageDispatcher : IBonMessageDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BonMessagingOptions _options;

    public InMemoryBonMessageDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _options = _serviceProvider.GetRequiredService<BonMessagingOptions>();
    }

    public async Task SendAsync<TMessage>(string serviceName, TMessage message,
        CancellationToken cancellationToken = default)
        where TMessage : IBonMessage
    {
        var consumerRegistration = _options.GetConsumerRegistrations()
            .FirstOrDefault(cr =>
                cr.ServiceType == typeof(IBonMessageConsumer<TMessage>) && cr.ServiceName == serviceName);

        if (consumerRegistration != null)
        {
            if (_serviceProvider.GetService(consumerRegistration.ServiceType) is IBonMessageConsumer<TMessage> consumer)
            {
                await consumer.ConsumeAsync(message, cancellationToken);
            }
        }
    }

    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IBonMessage
    {
        var consumerRegistrations = _options.GetConsumerRegistrations()
            .Where(cr => cr.ServiceType == typeof(IBonMessageConsumer<TMessage>));

        var tasks = new List<Task>();
        foreach (var cr in consumerRegistrations)
        {
            if (_serviceProvider.GetService(cr.ServiceType) is IBonMessageConsumer<TMessage> consumer)
            {
                tasks.Add(consumer.ConsumeAsync(message, cancellationToken));
            }
        }

        await Task.WhenAll(tasks);
    }
}