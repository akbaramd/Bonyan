using System.Collections.Concurrent;
using System.Text;
using Bonyan.Messaging.Abstractions;
using Newtonsoft.Json;

namespace Bonyan.Messaging.RabbitMQ
{
    internal class RabbitMqBus : IBonRabbitMqBus
    {
        private readonly IBonMessageProducer _producer;
        private readonly IBonMessageSubscriber _messageSubscriber;
        private readonly IServiceProvider _serviceProvider;
        private readonly BonServiceManager _serviceManager;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _responseHandlers;

        public RabbitMqBus(
            IBonMessageProducer producer,
            IServiceProvider serviceProvider,
            BonServiceManager serviceManager,
            IBonMessageSubscriber messageSubscriber)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
            _messageSubscriber = messageSubscriber ?? throw new ArgumentNullException(nameof(messageSubscriber));

            // Initialize response handlers for request-response pattern
            _responseHandlers = new ConcurrentDictionary<string, TaskCompletionSource<byte[]>>();
        }

        /// <summary>
        /// Sends a message and waits for a response.
        /// </summary>
        public async Task<TResponse> SendAsync<TMessage, TResponse>(
            string serviceName,
            TMessage message,
            IDictionary<string, object>? headers = null,
            string? correlationId = null,
            CancellationToken cancellationToken = default)
            where TMessage : class where TResponse : class
        {
            ValidateServiceName(serviceName);

            // Generate a unique correlation ID
            correlationId ??= Guid.NewGuid().ToString();

            // Create a temporary queue for response
            var replyQueueName = Guid.NewGuid().ToString();

            // Register a consumer to listen for the response
            var tcs = CreateTaskCompletionSource(correlationId);
            RegisterTemporaryConsumer<TResponse>(replyQueueName, correlationId, tcs);

            // Send the message using the producer
            await _producer.PublishAsync(
                serviceName: serviceName,
                message: message,
                headers: headers,
                replyQueue: replyQueueName,
                correlationId: correlationId,
                cancellationToken: cancellationToken);

            // Wait for the response with a timeout
            return await WaitForResponse<TResponse>(tcs, cancellationToken, correlationId);
        }

        /// <summary>
        /// Sends a message without waiting for a response.
        /// </summary>
        public Task SendAsync<TMessage>(
            string serviceName,
            TMessage message,
            IDictionary<string, object>? headers = null,
            string? correlationId = null,
            CancellationToken cancellationToken = default)
            where TMessage : class
        {
            ValidateServiceName(serviceName);

            correlationId ??= Guid.NewGuid().ToString();

            return _producer.PublishAsync(
                serviceName: serviceName,
                message: message,
                headers: headers,
                correlationId: correlationId,
                replyQueue:_serviceManager.ServiceId,
                cancellationToken: cancellationToken);
        }

     
        private void RegisterTemporaryConsumer<TMessage>(
            string queueName,
            string correlationId,
            TaskCompletionSource<byte[]> tcs)
            where TMessage : class
        {
            _messageSubscriber.Subscribe<TMessage>(queueName, async ctx =>
            {
                if (ctx.CorrelationId == correlationId)
                {
                    tcs.SetResult(SerializeMessage(ctx.Message));
                }
            },true);
        }

        private TaskCompletionSource<byte[]> CreateTaskCompletionSource(string correlationId)
        {
            var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
            _responseHandlers[correlationId] = tcs;
            return tcs;
        }

        private async Task<TResponse> WaitForResponse<TResponse>(
            TaskCompletionSource<byte[]> tcs,
            CancellationToken cancellationToken,
            string correlationId)
            where TResponse : class
        {
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(30)); // Timeout for response
                var result = await tcs.Task.WaitAsync(cts.Token);
                return DeserializeMessage<TResponse>(result);
            }
            finally
            {
                _responseHandlers.TryRemove(correlationId, out _);
            }
        }

        private T DeserializeMessage<T>(byte[] body)
        {
            var json = Encoding.UTF8.GetString(body);
            return JsonConvert.DeserializeObject<T>(json)!;
        }

        private byte[] SerializeMessage<T>(T message)
        {
            var json = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(json);
        }

        private void ValidateServiceName(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
                throw new ArgumentException("Service name cannot be null or empty.", nameof(serviceName));
        }

        public Task PublishAsync<TMessage>(
            TMessage message,
            IDictionary<string, object>? headers = null,
            string? correlationId = null,
            CancellationToken cancellationToken = default)
            where TMessage : class
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            return _producer.PublishAsync(
                "*",
                message: message,
                headers: headers,
                correlationId: correlationId,
                replyQueue:_serviceManager.ServiceId,
                cancellationToken: cancellationToken);
        }
    }
}
