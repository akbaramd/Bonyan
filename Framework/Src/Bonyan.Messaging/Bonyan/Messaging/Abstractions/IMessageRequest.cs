using Bonyan.Mediators;

namespace Bonyan.Messaging.Abstractions;

/// <summary>
/// Represents a message request that does not expect a response.
/// This interface extends IBonCommand to provide messaging-specific capabilities.
/// </summary>
public interface IMessageRequest : IBonCommand
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
    /// Gets or sets the timestamp when the request was created.
    /// </summary>
    DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the source service that created this request.
    /// </summary>
    string? SourceService { get; set; }

    /// <summary>
    /// Gets or sets the target service name for routing.
    /// </summary>
    string? TargetService { get; set; }
}

/// <summary>
/// Represents a message request that expects a response.
/// This interface extends IBonCommand to provide messaging-specific capabilities.
/// </summary>
/// <typeparam name="TResponse">The type of the expected response.</typeparam>
public interface IMessageRequest<TResponse> : IMessageRequest, IBonCommand<TResponse>
{
}
