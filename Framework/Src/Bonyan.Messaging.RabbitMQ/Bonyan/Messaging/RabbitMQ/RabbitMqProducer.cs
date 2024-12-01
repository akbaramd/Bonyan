using System.Text;
using Bonyan.Messaging.Abstractions;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Bonyan.Messaging.RabbitMQ
{
    internal class RabbitMqProducer : IBonRabbitMqProducer, IDisposable
    {
        private readonly IModel _channel;

        public RabbitMqProducer(IConnection connection)
        {
            _channel = connection?.CreateModel() ?? throw new ArgumentNullException(nameof(connection));

            // Enable Publisher Confirms
            _channel.ConfirmSelect();
        }

        public async Task PublishAsync<TMessage>(
            string serviceName,
            TMessage message,
            IDictionary<string, object>? headers = null,
            string? correlationId = null,
            string? replyQueue = null,
            CancellationToken cancellationToken = default)
            where TMessage : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageBody = SerializeMessage(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true; // Ensure message persistence
            properties.CorrelationId = correlationId ?? Guid.NewGuid().ToString();
            properties.Headers = headers;

            if (!string.IsNullOrEmpty(replyQueue))
                properties.ReplyTo = replyQueue;

            var routingKey = $"{serviceName}.{message.GetType().FullName}".ToLowerInvariant();

            // Publish the message
            _channel.BasicPublish(
                exchange: BonMessagingConst.DefaultExchangeName,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: messageBody);

            // Wait for publisher confirms
            _channel.WaitForConfirmsOrDie();

            await Task.CompletedTask;
        }

        private byte[] SerializeMessage<TMessage>(TMessage message)
        {
            var json = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(json);
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
