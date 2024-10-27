namespace Bonyan.DomainDrivenDesign.Domain.Events
{
  /// <summary>
  /// Represents a handler for domain events.
  /// </summary>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
  {
    /// <summary>
    /// Handles the specified domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Handle(TEvent domainEvent);
  }
}
