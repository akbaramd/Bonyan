﻿namespace Bonyan.DomainDrivenDesign.Domain.Entities;

/// <summary>
/// Base class for all entities in the domain, providing a method to retrieve the entity's keys.
/// </summary>
public abstract class Entity : IEntity
{
  /// <summary>
  /// Gets the array of keys that uniquely identify the entity.
  /// </summary>
  /// <returns>An array of objects representing the entity's keys.</returns>
  public abstract object[] GetKeys();
}

/// <summary>
/// Base class for entities with a strongly-typed key.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
  /// <summary>
  /// Gets or sets the identifier for the entity.
  /// </summary>
  public TKey Id { get; set; } = default!; // Ensure non-nullability

  /// <summary>
  /// Gets the array of keys that uniquely identify the entity.
  /// </summary>
  /// <returns>An array containing the entity's key.</returns>
  /// <exception cref="InvalidOperationException">Thrown if the Id is null or default.</exception>
  public override object[] GetKeys()
  {
    if (Id is null || Id.Equals(default(TKey)))
    {
      throw new InvalidOperationException($"{nameof(Id)} cannot be null or default.");
    }

    return new object[] { Id };
  }
}
