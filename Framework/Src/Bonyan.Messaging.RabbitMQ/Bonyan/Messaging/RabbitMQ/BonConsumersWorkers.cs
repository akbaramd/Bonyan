using Bonyan.Messaging.Abstractions;
using Bonyan.Workers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bonyan.Messaging.RabbitMQ;

public class BonConsumersWorkers : IBonWorker
{
    private readonly IBonMessageBus _messageBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly BonApplicationCreationOptions _options;

    public BonConsumersWorkers(IBonMessageBus messageBus, IServiceProvider serviceProvider, BonApplicationCreationOptions options)
    {
        _messageBus = messageBus;
        _serviceProvider = serviceProvider;
        _options = options;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var consumerInterfaceType = typeof(IBonMessageConsumer<>);
        var consumers = _serviceProvider.GetServices<object>() // Resolve all services
            .Where(service => service.GetType().GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == consumerInterfaceType))
            .ToList();

        foreach (var consumer in consumers)
        {
            // Identify the IBonMessageConsumer<TMessage> interface and extract TMessage
            var consumerInterface = consumer.GetType().GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == consumerInterfaceType);
            var messageType = consumerInterface.GenericTypeArguments[0];

            // Subscribe to the message bus
            RegisterConsumer(consumer, messageType);
        }
    }

    private void RegisterConsumer(object consumer, Type messageType)
    {
        var subscribeMethod = typeof(IBonMessageBus)
            .GetMethod("Subscribe")
            ?.MakeGenericMethod(messageType);

        if (subscribeMethod == null)
            throw new InvalidOperationException($"Subscribe method not found on {nameof(IBonMessageBus)}.");

        subscribeMethod.Invoke(_messageBus, new object[]
        {
            _options.ApplicationName,
            CreateMessageHandler(consumer, messageType)
        });
    }

    private Func<object, Task> CreateMessageHandler(object consumer, Type messageType)
    {
        var consumeMethod = consumer.GetType().GetMethod("ConsumeAsync", new[] { typeof(BonMessageContext<>).MakeGenericType(messageType), typeof(CancellationToken) });

        if (consumeMethod == null)
            throw new InvalidOperationException($"Consumer {consumer.GetType().Name} does not have a valid ConsumeAsync method.");

        return async (message) =>
        {
            var context = Activator.CreateInstance(typeof(BonMessageContext<>).MakeGenericType(messageType), message);
            var token = CancellationToken.None; // Replace with meaningful token if available
            await (Task)consumeMethod.Invoke(consumer, new[] { context, token });
        };
    }
}
