using System.Collections.Concurrent;
using System.Text;
using Bonyan.Messaging.Abstractions;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Bonyan.Messaging.RabbitMQ
{
    internal class RabbitMqSubscriber : IBonRabbitMqSubscriber, IDisposable
    {
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, AsyncEventingBasicConsumer> _consumers;

        public RabbitMqSubscriber(IConnection connection, IServiceProvider serviceProvider)
        {
            _channel = connection?.CreateModel() ?? throw new ArgumentNullException(nameof(connection));
            _serviceProvider = serviceProvider;
            _consumers = new ConcurrentDictionary<string, AsyncEventingBasicConsumer>();

            // Enable QoS
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);
        }

        public void Subscribe<TMessage>(
            string queueName,
            Func<BonMessageContext<TMessage>, Task> handler,
            bool isTemporary = false)
            where TMessage : class
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty.", nameof(queueName));

            // Declare the queue with appropriate settings
            var queueDeclareArgs = isTemporary
                ? new { exclusive = true, autoDelete = true, durable = false }
                : new { exclusive = false, autoDelete = false, durable = true };

            _channel.QueueDeclare(
                queue: queueName,
                durable: queueDeclareArgs.durable,
                exclusive: queueDeclareArgs.exclusive,
                autoDelete: queueDeclareArgs.autoDelete);

            // Bind the queue
            var routingKey = $"{queueName}.{typeof(TMessage).FullName}".ToLowerInvariant();
            _channel.QueueBind(queue: queueName, exchange: BonMessagingConst.DefaultExchangeName, routingKey: routingKey);
            var broadcastRoutingKey = $"*.{typeof(TMessage).FullName}".ToLowerInvariant();
            _channel.QueueBind(queue: queueName, exchange: BonMessagingConst.DefaultExchangeName, routingKey: broadcastRoutingKey);

            // Create a consumer
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                // Pass the message to the handler
                await OnMessageReceived<TMessage>(model, ea, handler);
            };

            // Store the consumer to be able to dispose of it later
            _consumers[queueName] = consumer;

            // Start consuming messages
            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        // Define the async OnMessageReceived method with the correct signature
        private async ValueTask OnMessageReceived<TMessage>(object model, BasicDeliverEventArgs ea, Func<BonMessageContext<TMessage>, Task> handler)
        {
            try
            {
                var message = DeserializeMessage<TMessage>(ea.Body.ToArray());
                var context = new BonMessageContext<TMessage>(
                    message,
                    correlationId: ea.BasicProperties.CorrelationId ?? string.Empty,
                    headers: ea.BasicProperties.Headers?.ToDictionary(k => k.Key, v => (object)v.Value),
                    replyTo: ea.BasicProperties.ReplyTo ?? string.Empty,
                    serviceProvider: _serviceProvider);

                await handler(context);

                // Acknowledge the message
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");

                // Optionally, reject the message to requeue
                _channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: true);
            }
        }

        private T DeserializeMessage<T>(byte[] body)
        {
            var json = Encoding.UTF8.GetString(body);
            return JsonConvert.DeserializeObject<T>(json)!;
        }

        public void Dispose()
        {
            // Dispose of all consumers to ensure they are properly unsubscribed and resources are released
            foreach (var consumer in _consumers.Values)
            {
                consumer.Received -= async (model, ea) =>
                {
                    await OnMessageReceived<object>(model, ea, null!);
                };
            }

            _channel?.Dispose();
        }
    }
}
