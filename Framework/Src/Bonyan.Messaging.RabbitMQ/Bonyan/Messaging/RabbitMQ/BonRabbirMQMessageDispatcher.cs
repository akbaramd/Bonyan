using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.DependencyInjection;
using Bonyan.Messaging.Abstractions;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Bonyan.Messaging.RabbitMQ
{
    public class BonRabbitMqMessageBus : IBonMessageBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQOptions _options;
        private readonly string _serviceName;
        private readonly string _exchangeName;

        public BonRabbitMqMessageBus(
            RabbitMQOptions options,
            IBonObjectAccessor<BonServiceOptions> serviceOptions)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _serviceName = serviceOptions?.Value?.ServiceName 
                           ?? throw new ArgumentNullException(nameof(serviceOptions), "Service name cannot be null.");

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(60)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _exchangeName = $"{_serviceName}.exchange";

            // Declare the exchange specific to the service
            _channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );
        }

        public Task SendAsync<TMessage>(
            string routingKey,
            TMessage message,
            CancellationToken cancellationToken = default) where TMessage : IBonMessage
        {
            if (message == null) 
                throw new ArgumentNullException(nameof(message));

            var body = SerializeMessage(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.Type = typeof(TMessage).FullName;

            // Use service name and provided routing key
            var fullRoutingKey = $"{_serviceName}.{routingKey}";

            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: fullRoutingKey,
                mandatory: false,
                basicProperties: properties,
                body: body
            );

            return Task.CompletedTask;
        }

        public Task PublishAsync<TMessage>(
            TMessage message,
            CancellationToken cancellationToken = default) where TMessage : IBonMessage
        {
            if (message == null) 
                throw new ArgumentNullException(nameof(message));

            // Use message type name as the routing key
            var routingKey = $"{_serviceName}.{typeof(TMessage).Name}";

            return SendAsync(routingKey, message, cancellationToken);
        }

        private byte[] SerializeMessage<TMessage>(TMessage message)
        {
            var json = JsonSerializer.Serialize(message);
            return Encoding.UTF8.GetBytes(json);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
