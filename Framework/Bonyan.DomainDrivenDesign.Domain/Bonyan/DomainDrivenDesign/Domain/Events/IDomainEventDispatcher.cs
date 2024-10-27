using Bonyan.DomainDrivenDesign.Domain.Aggregates;

namespace Bonyan.DomainDrivenDesign.Domain.Events
{
  /// <summary>
  /// Interface for domain event dispatchers, responsible for dispatching and clearing domain events from aggregates.
  /// </summary>
  public interface IDomainEventDispatcher
  {
    /// <summary>
    /// Dispatches all domain events for the given aggregates and clears the events after they are dispatched.
    /// </summary>
    /// <param name="aggregatesWithEvents">The aggregates containing the domain events to be dispatched.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DispatchAndClearEvents(IEnumerable<IAggregateRoot> aggregatesWithEvents);
  }
}
