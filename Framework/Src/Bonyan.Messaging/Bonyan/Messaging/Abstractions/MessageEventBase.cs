using Bonyan.Mediators;

namespace Bonyan.Messaging.Abstractions;

/// <summary>
/// Base implementation of IMessageEvent providing common functionality.
/// </summary>
public abstract class MessageEventBase : IMessageEvent
{
    /// <summary>
    /// Gets or sets the correlation ID for tracking related messages.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets additional headers for the message.
    /// </summary>
    public IDictionary<string, object>? Headers { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the event was created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the source service that created this event.
    /// </summary>
    public string? SourceService { get; set; }

    /// <summary>
    /// Initializes a new instance of MessageEventBase.
    /// </summary>
    protected MessageEventBase()
    {
        CorrelationId = Guid.NewGuid().ToString();
        Headers = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of MessageEventBase with specified correlation ID.
    /// </summary>
    /// <param name="correlationId">The correlation ID for tracking related messages.</param>
    protected MessageEventBase(string? correlationId)
    {
        CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        Headers = new Dictionary<string, object>();
    }
}
