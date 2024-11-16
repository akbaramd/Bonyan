using Bonyan.Layer.Domain.Aggregate.Abstractions;
using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Events;

namespace Bonyan.Layer.Domain.Aggregate;

/// <summary>
///     Represents the base class for aggregate roots in the domain.
/// </summary>
public abstract class BonAggregateRoot : BonEntity, IBonAggregateRoot
{
    private readonly List<IBonDomainEvent> _domainEvents = new();

    /// <summary>
    ///     Gets the list of domain events associated with this aggregate.
    /// </summary>
    public IReadOnlyCollection<IBonDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearEvents()
    {
        _domainEvents.Clear();
        ;
    }

    /// <summary>
    ///     Registers a domain event for this aggregate.
    /// </summary>
    /// <param name="bonDomainEvent">The domain event to register.</param>
    protected void AddDomainEvent(BonDomainEventBase bonDomainEvent)
    {
        _domainEvents.Add(bonDomainEvent);
    }

    /// <summary>
    ///     Clears all domain events for this aggregate.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    ///     Removes a specific domain event.
    /// </summary>
    /// <param name="bonDomainEvent">The domain event to remove.</param>
    protected void RemoveDomainEvent(IBonDomainEvent bonDomainEvent)
    {
        _domainEvents.Remove(bonDomainEvent);
    }
}

/// <summary>
///     Represents the base class for aggregate roots with a strongly-typed key.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public abstract class BonAggregateRoot<TKey> : BonEntity<TKey>, IBonAggregateRoot<TKey>
{
    private readonly List<IBonDomainEvent> _domainEvents = new();

    /// <summary>
    ///     Gets the list of domain events associated with this aggregate.
    /// </summary>
    public IReadOnlyCollection<IBonDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    ///     Registers a domain event for this aggregate.
    /// </summary>
    /// <param name="bonDomainEvent">The domain event to register.</param>
    protected void AddDomainEvent(IBonDomainEvent bonDomainEvent)
    {
        _domainEvents.Add(bonDomainEvent);
    }

    /// <summary>
    ///     Clears all domain events for this aggregate.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    ///     Removes a specific domain event.
    /// </summary>
    /// <param name="bonDomainEvent">The domain event to remove.</param>
    protected void RemoveDomainEvent(IBonDomainEvent bonDomainEvent)
    {
        _domainEvents.Remove(bonDomainEvent);
    }
}