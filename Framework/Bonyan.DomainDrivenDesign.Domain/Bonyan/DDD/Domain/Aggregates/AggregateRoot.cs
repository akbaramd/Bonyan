using Bonyan.DDD.Domain.Entities;

namespace Bonyan.DDD.Domain.Aggregates;

/// <summary>
/// Represents the base class for aggregate roots in the domain.
/// </summary>
public abstract class AggregateRoot : Entity, IAggregateRoot
{
  // This class inherits everything from Entity and implements IAggregateRoot
  // No additional members are required unless specific logic is needed for non-generic aggregate roots.
}

/// <summary>
/// Represents the base class for aggregate roots with a strongly-typed key.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
{
  // This class inherits everything from Entity<TKey> and implements IAggregateRoot
  // No additional members are required unless specific logic is needed for generic aggregate roots.
}
