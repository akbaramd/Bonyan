using Bonyan.Messaging.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bonyan.Messaging.OutBox.HostedServices;

public class OutboxProcessorHostedService : BackgroundService
{
    private readonly IOutboxStore _outboxStore;
    private readonly IBonMessageProducer _messageProducer;
    private readonly ILogger<OutboxProcessorHostedService> _logger;

    public OutboxProcessorHostedService(
        IOutboxStore outboxStore,
        IBonMessageProducer messageProducer,
        ILogger<OutboxProcessorHostedService> logger)
    {
        _outboxStore = outboxStore ?? throw new ArgumentNullException(nameof(outboxStore));
        _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxProcessorHostedService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await _outboxStore.GetPendingMessagesAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        // Use the saved message type for deserialization
                        var payload = DeserializeMessage(message.Payload, message.MessageType);

                        await _messageProducer.PublishAsync(
                            serviceName: message.Destination,
                            message: payload,
                            headers: JsonConvert.DeserializeObject<IDictionary<string, object>>(message.Headers),
                            correlationId: message.CorrelationId,
                            replyQueue: message.ReplyQueueName,
                            cancellationToken: stoppingToken);

                        await _outboxStore.DeleteAsync(message.Id, stoppingToken);
                        _logger.LogInformation("Processed outbox message with ID {MessageId}.", message.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process outbox message with ID {MessageId}.", message.Id);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // This is expected when the app is shutting down.
                _logger.LogInformation("OutboxProcessorHostedService is being canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in OutboxProcessorHostedService.");
            }
        }

        _logger.LogInformation("OutboxProcessorHostedService stopped.");
    }

    private object DeserializeMessage(string payload, string messageType)
    {
        var type = Type.GetType(messageType);
        if (type == null)
        {
            throw new InvalidOperationException($"Failed to resolve type {messageType}.");
        }

        return JsonConvert.DeserializeObject(payload, type)
               ?? throw new InvalidOperationException("Failed to deserialize message payload.");
    }
}
