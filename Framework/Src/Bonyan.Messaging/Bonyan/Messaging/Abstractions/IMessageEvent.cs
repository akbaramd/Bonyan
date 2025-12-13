using Bonyan.Mediators;

namespace Bonyan.Messaging.Abstractions;

/// <summary>
/// Represents a message event that does not expect a response.
/// This interface extends IBonEvent to provide messaging-specific capabilities for events.
/// </summary>
public interface IMessageEvent : IBonEvent
{
    /// <summary>
    /// Gets or sets the correlation ID for tracking related messages.
    /// </summary>
    string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets additional headers for the message.
    /// </summary>
    IDictionary<string, object>? Headers { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the event was created.
    /// </summary>
    DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the source service that created this event.
    /// </summary>
    string? SourceService { get; set; }
}
