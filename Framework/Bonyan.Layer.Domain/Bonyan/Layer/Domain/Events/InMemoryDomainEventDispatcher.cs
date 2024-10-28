using Bonyan.Layer.Domain.Aggregates;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Layer.Domain.Events
{
  /// <summary>
  /// In-memory implementation of IDomainEventDispatcher, useful for testing and lightweight scenarios.
  /// </summary>
  public class InMemoryDomainEventDispatcher : IDomainEventDispatcher
  {
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryDomainEventDispatcher"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving event handlers.</param>
    public InMemoryDomainEventDispatcher(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Dispatches all domain events for the given aggregates and clears the events after they are dispatched.
    /// </summary>
    /// <param name="aggregatesWithEvents">The aggregates containing the domain events to be dispatched.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DispatchAndClearEvents(IEnumerable<IAggregateRoot> aggregatesWithEvents)
    {
      foreach (var aggregate in aggregatesWithEvents)
      {
        var domainEvents = aggregate.DomainEvents.ToList();
        aggregate.ClearEvents();

        foreach (var domainEvent in domainEvents)
        {
          await DispatchEvent(domainEvent);
        }
      }
    }

    /// <summary>
    /// Dispatches a domain event to the appropriate handlers.
    /// </summary>
    /// <param name="domainEvent">The domain event to dispatch.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task DispatchEvent<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent

    {
      var handler = _serviceProvider.GetRequiredService<IDomainEventHandler<TEvent>>();
      await handler.Handle(domainEvent);
    }
  }
}
