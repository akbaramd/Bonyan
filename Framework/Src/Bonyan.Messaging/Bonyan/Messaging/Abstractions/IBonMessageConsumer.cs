namespace Bonyan.Messaging.Abstractions;

/// <summary>
/// Represents a handler for domain events or commands without a response.
/// </summary>
/// <typeparam name="TMessage">The type of the event or command.</typeparam>
public interface IBonMessageConsumer<TMessage> : IBonMessageConsumer where TMessage : class
{
    Task ConsumeAsync(BonMessageContext<TMessage> context, CancellationToken cancellationToken);
}


/// <summary>
/// Represents a non-generic consumer base interface.
/// </summary>
public interface IBonMessageConsumer
{
    // Shared logic for non-generic consumers, if needed.
}