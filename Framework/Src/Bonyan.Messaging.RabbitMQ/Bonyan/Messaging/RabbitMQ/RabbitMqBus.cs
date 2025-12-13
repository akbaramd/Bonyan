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
        /// ارسال درخواست به سرویس مشخص و انتظار برای پاسخ از طریق RabbitMQ.
        /// </summary>
        public async Task<TResponse> SendAsync<TRequest, TResponse>(
            string destinationServiceName,
            TRequest request,
            CancellationToken cancellationToken = default)
            where TRequest : class, IMessageRequest<TResponse>
            where TResponse : class
        {
            ValidateRequest(request);
            ValidateServiceName(destinationServiceName);

            // Generate a unique correlation ID if not provided
            request.CorrelationId ??= Guid.NewGuid().ToString();
            request.TargetService = destinationServiceName;

            // Create a temporary queue for response
            var replyQueueName = Guid.NewGuid().ToString();

            // Register a consumer to listen for the response
            var tcs = CreateTaskCompletionSource(request.CorrelationId);
            RegisterTemporaryConsumer<TResponse>(replyQueueName, request.CorrelationId, tcs);

            // Send the message using the producer
            await _producer.PublishAsync(
                serviceName: destinationServiceName,
                message: request,
                headers: request.Headers,
                replyQueue: replyQueueName,
                correlationId: request.CorrelationId,
                cancellationToken: cancellationToken);

            // Wait for the response with a timeout
            return await WaitForResponse<TResponse>(tcs, cancellationToken, request.CorrelationId);
        }

        /// <summary>
        /// ارسال درخواست به سرویس مشخص بدون انتظار برای پاسخ از طریق RabbitMQ.
        /// </summary>
        public Task SendAsync<TRequest>(
            string destinationServiceName,
            TRequest request,
            CancellationToken cancellationToken = default)
            where TRequest : class, IMessageRequest
        {
            ValidateRequest(request);
            ValidateServiceName(destinationServiceName);

            request.CorrelationId ??= Guid.NewGuid().ToString();
            request.TargetService = destinationServiceName;

            return _producer.PublishAsync(
                serviceName: destinationServiceName,
                message: request,
                headers: request.Headers,
                correlationId: request.CorrelationId,
                replyQueue: _serviceManager.ServiceId,
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

        private void ValidateRequest<TRequest>(TRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
        }

        private void ValidateServiceName(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
                throw new ArgumentException("Service name cannot be null or empty.", nameof(serviceName));
        }

        public Task PublishAsync(
            IMessageEvent messageEvent,
            CancellationToken cancellationToken = default)
        {
            if (messageEvent == null) throw new ArgumentNullException(nameof(messageEvent));

            return _producer.PublishAsync(
                "*",
                message: messageEvent,
                headers: messageEvent.Headers,
                correlationId: messageEvent.CorrelationId,
                replyQueue: _serviceManager.ServiceId,
                cancellationToken: cancellationToken);
        }
    }
}
