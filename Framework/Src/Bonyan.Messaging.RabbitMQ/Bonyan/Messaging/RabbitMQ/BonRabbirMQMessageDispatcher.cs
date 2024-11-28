using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Bonyan.Messaging.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;

namespace Bonyan.Messaging.Abstractions;

public class RabbitMQMessageBus : IBonMessageBus, IDisposable
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly string _topicExchangeName;
    private readonly BonServiceManager _serviceManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _responseHandlers;

    public RabbitMQMessageBus(IOptions<RabbitMQOptions> options, BonServiceManager applicationCreationOptions, IServiceProvider serviceProvider)
    {
        _serviceManager = applicationCreationOptions;
        _serviceProvider = serviceProvider;

        var rabbitOptions = options.Value;
        _connection = CreateConnection(rabbitOptions);
        _channel = _connection.CreateModel();

        // Declare the topic exchange for publish/subscribe and direct routing
        _topicExchangeName = $"bonyan.topic";
        _channel.ExchangeDeclare(_topicExchangeName, ExchangeType.Topic, durable: true);

        // Set prefetch count to ensure fair message dispatch
        _channel.BasicQos(0, 1, false);

        // Initialize the response handlers for request-response pattern
        _responseHandlers = new ConcurrentDictionary<string, TaskCompletionSource<byte[]>>();
    }

    /// <summary>
    /// Sends a message to a specific service and waits for a response.
    /// </summary>
    public async Task<TResponse> SendAsync<TMessage, TResponse>(
        string serviceName,
        TMessage message,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,
        bool isReply = false,
        CancellationToken cancellationToken = default)
        where TMessage : class where TResponse : class
    {
        ValidateServiceName(serviceName);
        serviceName = NormalizeName(serviceName);
        correlationId ??= Guid.NewGuid().ToString(); // Generate correlation ID if not provided

        // Create a TaskCompletionSource to await the response
        var tcs = CreateTaskCompletionSource(correlationId);

        // Declare a temporary reply queue to receive the response
        var replyQueueName = CreateTemporaryReplyQueue(correlationId);

        // Bind the reply queue for direct responses
        BindQueue(replyQueueName, $"{typeof(TResponse).Name}.{replyQueueName}.direct");

        // Set up a consumer to listen to the reply queue
        var replyConsumer = CreateReplyConsumer(replyQueueName, correlationId, tcs);
        
        var suffix = isReply ? $"{serviceName}.{correlationId}.direct" : $"{serviceName}.direct";
        // Publish the message with a routing key for the specific service
        PublishMessage(
            message: message,
            messageType: typeof(TMessage).Name,
            routingKeySuffix: suffix,
            headers: headers,
            correlationId: correlationId,
            replyQueueName: replyQueueName
        );

        // Wait for the response with a timeout
        return await WaitForResponse<TResponse>(tcs, cancellationToken, correlationId);
    }

    /// <summary>
    /// Sends a message to a specific service without expecting a response.
    /// </summary>
    public Task SendAsync<TMessage>(
        string serviceName,
        TMessage message,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,
        bool isReply = false,
        CancellationToken cancellationToken = default)
        where TMessage : class
    {
        ValidateServiceName(serviceName);
        serviceName = NormalizeName(serviceName);

        var suffix =   $"{serviceName}.direct";
        // Publish the message to the specified service's queue
        PublishMessage(
            message: message,
            messageType: typeof(TMessage).Name,
            routingKeySuffix: suffix,
            headers: headers,
            correlationId: correlationId
        );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Publishes a message to all subscribers.
    /// </summary>
    public Task PublishAsync<TMessage>(
        TMessage message,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        // Publish the message to all subscribers listening to the message type
        PublishMessage(
            message: message,
            messageType: typeof(TMessage).Name,
            routingKeySuffix: _serviceManager.ServiceId+".broadcast",
            headers: headers,
            correlationId: correlationId
        );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Subscribes to a specific message type.
    /// </summary>
    public void Subscribe<TMessage>(string queueName, Func<BonMessageContext<TMessage>, Task> handler)
        where TMessage : class
    {
        queueName = NormalizeName(queueName);

        // Declare the queue to ensure it exists
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

        // Bind the queue to listen to both direct and broadcast messages
        BindQueue(queueName, $"{typeof(TMessage).Name}.{queueName}.direct");
        BindQueue(queueName, $"{typeof(TMessage).Name}.*.broadcast");

        // Create a consumer to process incoming messages
        var consumer = CreateMessageConsumer(queueName, handler);
        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    private IConnection CreateConnection(RabbitMQOptions options)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password,
            VirtualHost = options.VirtualHost,
            DispatchConsumersAsync = true
        };

        return factory.CreateConnection();
    }

    private void ValidateServiceName(string serviceName)
    {
        if (string.IsNullOrEmpty(serviceName))
            throw new ArgumentException("Service name cannot be null or empty.", nameof(serviceName));
    }

    private string CreateTemporaryReplyQueue(string coreealtionId)
    {
        var replyQueueName = $"{_serviceManager.ServiceId}.reply.{coreealtionId}".ToLowerInvariant();
        _channel.QueueDeclare(queue: replyQueueName, durable: false, exclusive: true, autoDelete: true);
        return replyQueueName;
    }

    private TaskCompletionSource<byte[]> CreateTaskCompletionSource(string correlationId)
    {
        var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
        _responseHandlers[correlationId] = tcs;
        return tcs;
    }

    private void BindQueue(string queueName, string routingKey)
    {
  
        _channel.QueueBind(queue: queueName, exchange: _topicExchangeName, routingKey: NormalizeName(routingKey));
    }

    private AsyncEventingBasicConsumer CreateReplyConsumer(string replyQueueName, string correlationId, TaskCompletionSource<byte[]> tcs)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            if (ea.BasicProperties.CorrelationId == correlationId)
            {
                try
                {
                    tcs.SetResult(ea.Body.ToArray());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
                finally
                {
                    foreach (var tag in consumer.ConsumerTags)
                        _channel.BasicCancel(tag);
                }
            }
        };

        _channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);
        return consumer;
    }

    private AsyncEventingBasicConsumer CreateMessageConsumer<TMessage>(string queueName, Func<BonMessageContext<TMessage>, Task> handler) where TMessage : class
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var message = DeserializeMessage<TMessage>(ea.Body.ToArray());
                var correlationId = ea.BasicProperties.CorrelationId ?? string.Empty;
                var headers = ea.BasicProperties.Headers?.ToDictionary(k => k.Key, v => (object)v.Value) ?? new Dictionary<string, object>();
                var replyTo = ea.BasicProperties.ReplyTo ?? string.Empty;

                var context = new BonMessageContext<TMessage>(message, correlationId, headers, replyTo,_serviceProvider);
                await handler(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        };
        return consumer;
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
            var res = await tcs.Task.WaitAsync(cts.Token);
            return DeserializeMessage<TResponse>(res);
        }
        finally
        {
            _responseHandlers.TryRemove(correlationId, out _);
        }
    }

    private void PublishMessage<TMessage>(
        TMessage message,
        string messageType,
        string routingKeySuffix,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,
        string? replyQueueName = null)
        where TMessage : class
    {
        var props = _channel.CreateBasicProperties();
        props.CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        props.ReplyTo = replyQueueName;
        props.Headers = headers;

        var messageBody = SerializeMessage(message);
        
        var routingKey = $"{NormalizeName(messageType)}.{routingKeySuffix}";

        _channel.BasicPublish(
            exchange: _topicExchangeName,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: props,
            body: messageBody);
    }

    private string NormalizeName(string name)
    {
        return name.ToLowerInvariant().Trim();
    }

    private byte[] SerializeMessage<T>(T message)
    {
        var json = JsonSerializer.Serialize(message);
        return Encoding.UTF8.GetBytes(json);
    }

    private T DeserializeMessage<T>(byte[] body)
    {
        var json = Encoding.UTF8.GetString(body);
        return JsonSerializer.Deserialize<T>(json)!;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
