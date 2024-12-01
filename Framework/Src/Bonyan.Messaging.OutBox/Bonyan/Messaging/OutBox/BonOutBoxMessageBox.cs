using System.Collections.Concurrent;
using System.Text;
using Bonyan.Layer.Domain;
using Bonyan.Messaging.Abstractions;
using Newtonsoft.Json;

namespace Bonyan.Messaging.OutBox;

public class BonOutBoxMessageBox : IBonOutBoxBus
{
    private readonly IOutboxStore _outboxStore;
    private readonly IBonMessageSubscriber _messageSubscriber;
    private readonly BonServiceManager _bonServiceManager;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _responseHandlers;

    public BonOutBoxMessageBox(IOutboxStore outboxStore, IBonMessageSubscriber messageSubscriber, BonServiceManager bonServiceManager)
    {
        _outboxStore = outboxStore ?? throw new ArgumentNullException(nameof(outboxStore));
        _messageSubscriber = messageSubscriber ?? throw new ArgumentNullException(nameof(messageSubscriber));
        _bonServiceManager = bonServiceManager;
        _responseHandlers = new ConcurrentDictionary<string, TaskCompletionSource<byte[]>>();
    }

    #region Public Methods

    public async Task<TResponse> SendAsync<TMessage, TResponse>(
        string serviceName,
        TMessage message,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class where TResponse : class
    {
        ValidateServiceName(serviceName);

        // Ensure correlationId is generated if not provided
        correlationId ??= GenerateCorrelationId();
        var replyQueueName = Guid.NewGuid().ToString();

        var outboxMessage = CreateOutboxMessage(serviceName, message, headers,replyQueueName, correlationId);
        await _outboxStore.AddAsync(outboxMessage);

        var tcs = CreateTaskCompletionSource(correlationId);
        RegisterTemporaryConsumer<TResponse>(replyQueueName, correlationId, tcs);

        var response = await WaitForResponse<TResponse>(tcs, cancellationToken, correlationId);

        return response;
    }

    public async Task SendAsync<TMessage>(
        string serviceName,
        TMessage message,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class
    {
        // Ensure correlationId is generated if not provided
        correlationId ??= GenerateCorrelationId();

        var outboxMessage = CreateOutboxMessage(serviceName, message, headers, null,correlationId);
        await _outboxStore.AddAsync(outboxMessage);

    }

    public async Task PublishAsync<TMessage>(
        TMessage message,
        IDictionary<string, object>? headers = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
        where TMessage : class
    {
        // Ensure correlationId is generated if not provided
        correlationId ??= GenerateCorrelationId();

        var outboxMessage = CreateOutboxMessage(message.GetType().Name, message, headers, _bonServiceManager.ServiceId,correlationId);
        await _outboxStore.AddAsync(outboxMessage);
    }

    #endregion

    #region Private Methods

    private string GenerateCorrelationId()
    {
        return Guid.NewGuid().ToString();
    }

    private TaskCompletionSource<byte[]> CreateTaskCompletionSource(string correlationId)
    {
        var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
        _responseHandlers[correlationId] = tcs;
        return tcs;
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
        }, isTemporary: true);
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

    private BonOutboxMessage CreateOutboxMessage<TMessage>(
        string destination,
        TMessage message,
        IDictionary<string, object>? headers,
        string replyQueueName,
        string? correlationId)
        where TMessage : class
    {
        return BonOutboxMessage.Create(
            destination,
            JsonConvert.SerializeObject(message),
            message.GetType().AssemblyQualifiedName, // Include the fully qualified type name
            JsonConvert.SerializeObject(headers),
            replyQueueName,
            correlationId ?? Guid.NewGuid().ToString() // Generate correlation ID if not provided
        );

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

    #endregion
}
