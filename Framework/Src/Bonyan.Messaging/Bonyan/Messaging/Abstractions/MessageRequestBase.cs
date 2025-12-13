using Bonyan.Mediators;

namespace Bonyan.Messaging.Abstractions;

/// <summary>
/// Base implementation of IMessageRequest providing common functionality for requests without responses.
/// </summary>
public abstract class MessageRequestBase : IMessageRequest
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
    /// Gets or sets the timestamp when the request was created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the source service that created this request.
    /// </summary>
    public string? SourceService { get; set; }

    /// <summary>
    /// Gets or sets the target service name for routing.
    /// </summary>
    public string? TargetService { get; set; }

    /// <summary>
    /// Initializes a new instance of MessageRequestBase.
    /// </summary>
    protected MessageRequestBase()
    {
        CorrelationId = Guid.NewGuid().ToString();
        Headers = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of MessageRequestBase with specified correlation ID.
    /// </summary>
    /// <param name="correlationId">The correlation ID for tracking related messages.</param>
    protected MessageRequestBase(string? correlationId)
    {
        CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        Headers = new Dictionary<string, object>();
    }
}

/// <summary>
/// Base implementation of IMessageRequest providing common functionality for requests with responses.
/// </summary>
/// <typeparam name="TResponse">The type of the expected response.</typeparam>
public abstract class MessageRequestBase<TResponse> : MessageRequestBase, IMessageRequest<TResponse>
{
    /// <summary>
    /// Initializes a new instance of MessageRequestBase.
    /// </summary>
    protected MessageRequestBase() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of MessageRequestBase with specified correlation ID.
    /// </summary>
    /// <param name="correlationId">The correlation ID for tracking related messages.</param>
    protected MessageRequestBase(string? correlationId) : base(correlationId)
    {
    }
}
