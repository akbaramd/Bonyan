namespace Bonyan.Layer.Domain.Events
{
  /// <summary>
  ///     Represents a handler for domain events.
  /// </summary>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  public interface IDomainEventHandler<TEvent> : IDomainEventHandler where TEvent : IDomainEvent
    {
      /// <summary>
      ///     Handles the specified domain event.
      /// </summary>
      /// <param name="domainEvent">The domain event to handle.</param>
      /// <returns>A task that represents the asynchronous operation.</returns>
      Task Handle(TEvent domainEvent);
    }
}


namespace Bonyan.Layer.Domain.Events
{
    public interface IDomainEventHandler
    {
    }
}