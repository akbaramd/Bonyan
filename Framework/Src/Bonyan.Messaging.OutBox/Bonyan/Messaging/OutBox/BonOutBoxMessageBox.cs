using System.Collections.Concurrent;
using System.Text;
using Bonyan.Layer.Domain;
using Bonyan.Messaging.Abstractions;
using Newtonsoft.Json;

namespace Bonyan.Messaging.OutBox;

/// <summary>
/// Implementation of the Outbox pattern for reliable messaging.
/// This class stores messages in an outbox store and processes them asynchronously.
/// </summary>
public class BonOutBoxMessageBox : IBonOutBoxBus
{
    private readonly IOutboxStore _outboxStore;
    private readonly IBonMessageProducer _messageProducer;
    private readonly BonServiceManager _bonServiceManager;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _responseHandlers;

    public BonOutBoxMessageBox(
        IOutboxStore outboxStore, 
        IBonMessageProducer messageProducer, 
        BonServiceManager bonServiceManager)
    {
        _outboxStore = outboxStore ?? throw new ArgumentNullException(nameof(outboxStore));
        _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
        _bonServiceManager = bonServiceManager ?? throw new ArgumentNullException(nameof(bonServiceManager));
        _responseHandlers = new ConcurrentDictionary<string, TaskCompletionSource<byte[]>>();
    }

    #region IBonMessageBus Implementation

    /// <summary>
    /// Sends a message request to a specific service and waits for a response.
    /// The message is stored in the outbox and processed asynchronously.
    /// </summary>
    public async Task<TResponse> SendAsync<TRequest, TResponse>(
        string destinationServiceName,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class, IMessageRequest<TResponse>
        where TResponse : class
    {
        ValidateServiceName(destinationServiceName);

        // Generate correlation ID if not provided
        if (string.IsNullOrEmpty(request.CorrelationId))
        {
            request.CorrelationId = Guid.NewGuid().ToString();
        }

        var replyQueueName = Guid.NewGuid().ToString();
        var outboxMessage = CreateOutboxMessage(destinationServiceName, request, request.Headers, replyQueueName, request.CorrelationId);
        
        await _outboxStore.AddAsync(outboxMessage, cancellationToken);

        // For outbox pattern, we don't wait for immediate response
        // The response will be handled by the outbox processor
        // This is a fire-and-forget pattern with eventual consistency
        throw new NotImplementedException("Outbox pattern does not support synchronous request-response. Use fire-and-forget SendAsync instead.");
    }

    /// <summary>
    /// Sends a message request to a specific service without waiting for a response.
    /// The message is stored in the outbox and processed asynchronously.
    /// </summary>
    public async Task SendAsync<TRequest>(
        string destinationServiceName,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class, IMessageRequest
    {
        ValidateServiceName(destinationServiceName);

        // Generate correlation ID if not provided
        if (string.IsNullOrEmpty(request.CorrelationId))
        {
            request.CorrelationId = Guid.NewGuid().ToString();
        }

        var outboxMessage = CreateOutboxMessage(destinationServiceName, request, request.Headers, null, request.CorrelationId);
        await _outboxStore.AddAsync(outboxMessage, cancellationToken);
    }

    /// <summary>
    /// Publishes a message event to multiple subscribers.
    /// The message is stored in the outbox and processed asynchronously.
    /// </summary>
    public async Task PublishAsync(
        IMessageEvent messageEvent,
        CancellationToken cancellationToken = default)
    {
        if (messageEvent == null)
            throw new ArgumentNullException(nameof(messageEvent));

        // Generate correlation ID if not provided
        if (string.IsNullOrEmpty(messageEvent.CorrelationId))
        {
            messageEvent.CorrelationId = Guid.NewGuid().ToString();
        }

        var outboxMessage = CreateOutboxMessage(
            messageEvent.GetType().Name, 
            messageEvent, 
            messageEvent.Headers, 
            null, 
            messageEvent.CorrelationId);
            
        await _outboxStore.AddAsync(outboxMessage, cancellationToken);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates an outbox message from the provided parameters.
    /// </summary>
    private BonOutboxMessage CreateOutboxMessage<TMessage>(
        string destination,
        TMessage message,
        IDictionary<string, object>? headers,
        string? replyQueueName,
        string? correlationId)
        where TMessage : class
    {
        return BonOutboxMessage.Create(
            destination,
            JsonConvert.SerializeObject(message),
            message.GetType().AssemblyQualifiedName ?? message.GetType().Name,
            JsonConvert.SerializeObject(headers ?? new Dictionary<string, object>()),
            replyQueueName ?? string.Empty,
            correlationId ?? Guid.NewGuid().ToString()
        );
    }

    /// <summary>
    /// Validates that the service name is not null or empty.
    /// </summary>
    private static void ValidateServiceName(string serviceName)
    {
        if (string.IsNullOrEmpty(serviceName))
            throw new ArgumentException("Service name cannot be null or empty.", nameof(serviceName));
    }

    #endregion
}
