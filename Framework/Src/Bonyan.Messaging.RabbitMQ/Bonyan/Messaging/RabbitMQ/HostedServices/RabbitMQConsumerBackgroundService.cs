using System.Reflection;
using Bonyan.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bonyan.Messaging.RabbitMQ.HostedServices
{
    internal class RabbitMqConsumerBackgroundService : BackgroundService
    {
        private readonly IBonMessageSubscriber _messageSubscriber;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQCosnumerTypeAccessor _rabbitMqConsumerTypeAccessor;
        private readonly ILogger<RabbitMqConsumerBackgroundService> _logger;

        public RabbitMqConsumerBackgroundService( 
            IServiceProvider serviceProvider, 
            RabbitMQCosnumerTypeAccessor rabbitMqConsumerTypeAccessor, 
            IBonMessageSubscriber messageSubscriber,
            ILogger<RabbitMqConsumerBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _rabbitMqConsumerTypeAccessor = rabbitMqConsumerTypeAccessor;
            _messageSubscriber = messageSubscriber;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitMqConsumerBackgroundService started.");

            // Loop through all consumer mappings and register them
            foreach (var mapping in _rabbitMqConsumerTypeAccessor.GetMappings())
            {
                var consumer = _serviceProvider.GetRequiredService(mapping.ConsumerType);
                RegisterConsumer(consumer, mapping.QueueName, stoppingToken);
            }

            // Wait until the cancellation token is triggered (shut down)
            await Task.Delay(Timeout.Infinite, stoppingToken); 

            _logger.LogInformation("RabbitMqConsumerBackgroundService is stopping.");
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

            // Invoke the generic Handle method with the consumer and cancellation token
            handleMethod.Invoke(this, new object[] { queueName, consumer, stoppingToken });
        }

        private void Handle<TMessage>(string queue, IBonMessageConsumer<TMessage> consumer, CancellationToken token)
            where TMessage : class
        {
            // Subscribe to the message with cancellation token handling
            _messageSubscriber.Subscribe<TMessage>(queue, async context =>
            {
                // Check if cancellation is requested
                if (token.IsCancellationRequested) return;

                try
                {
                    // Handle the consumption of the message
                    _logger.LogInformation($"Processing message from queue: {queue}.");
                    await consumer.ConsumeAsync(context, token);
                    _logger.LogInformation($"Successfully processed message from queue: {queue}.");
                }
                catch (Exception ex)
                {
                    // Log any exceptions that occur during message processing
                    _logger.LogError(ex, "Error occurred while processing message.");
                }
            });
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping RabbitMQ Consumer Background Service...");

            // Cancel any ongoing subscriptions
            await base.StopAsync(cancellationToken);

            // Ensure cancellation is acknowledged
            _messageSubscriber.Dispose();

            _logger.LogInformation("RabbitMQ Consumer Background Service stopped.");
        }
    }
}
