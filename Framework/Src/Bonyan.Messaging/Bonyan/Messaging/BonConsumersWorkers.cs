using System.Reflection;
using Bonyan.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bonyan.Messaging;

public class BonBackgroundConsumerService : BackgroundService
{
    private readonly IBonMessageBus _messageBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly BonMessagingConfiguration _messagingConfiguration;

    public BonBackgroundConsumerService(IBonMessageBus messageBus, IServiceProvider serviceProvider, BonMessagingConfiguration messagingConfiguration)
    {
        _messageBus = messageBus;
        _serviceProvider = serviceProvider;
        _messagingConfiguration = messagingConfiguration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var registration in _messagingConfiguration.GetConsumerRegistrations())
        {
            var consumer = _serviceProvider.GetRequiredService(registration.ImplementationType);
            RegisterConsumer(consumer, registration.QueueName, stoppingToken);
        }

        await Task.CompletedTask; // Keep the background service alive
    }

    private void RegisterConsumer(object consumer, string queueName, CancellationToken stoppingToken)
    {
        var consumerInterface = consumer.GetType().GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBonMessageConsumer<>));

        if (consumerInterface == null)
            throw new InvalidOperationException($"Consumer {consumer.GetType().Name} does not implement IBonMessageConsumer<>");

        var messageType = consumerInterface.GenericTypeArguments[0];

        var handleMethod = GetType()
            .GetMethod(nameof(Handle), BindingFlags.NonPublic | BindingFlags.Instance)
            ?.MakeGenericMethod(messageType);

        if (handleMethod == null)
            throw new InvalidOperationException("Handle method not found.");

        handleMethod.Invoke(this, new object[] { queueName, consumer, stoppingToken });
    }

    private void Handle<TMessage>(string queue, IBonMessageConsumer<TMessage> consumer, CancellationToken token)
        where TMessage : class
    {
        _messageBus.Subscribe<TMessage>(queue, async context =>
        {
            if (token.IsCancellationRequested) return;
            await consumer.ConsumeAsync(context, token);
        });
    }
}
