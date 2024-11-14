namespace Bonyan.Layer.Domain.DomainEvent.Abstractions;

/// <summary>
///     Interface for domain event dispatchers, responsible for dispatching and clearing domain events from aggregates.
/// </summary>
public interface IBonDomainEventDispatcher
{
  /// <summary>
  ///     Dispatches all domain events for the given aggregates and clears the events after they are dispatched.
  /// </summary>
  /// <param name="domainEvent"></param>
  /// <param name="cancellationToken"></param>
  /// <param name="aggregatesWithEvents">The aggregates containing the domain events to be dispatched.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : IBonDomainEvent;
}