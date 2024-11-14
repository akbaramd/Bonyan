namespace Bonyan.Layer.Domain.Abstractions;

/// <summary>
///     Represents a handler for domain events.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public interface IBonDomainEventHandler<in TEvent> : IDomainEventHandler where TEvent : IBonDomainEvent
{
    /// <summary>
    ///     Handles the specified domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleAsync(TEvent domainEvent,CancellationToken? cancellationToken = default);
}


public interface IDomainEventHandler
{
}