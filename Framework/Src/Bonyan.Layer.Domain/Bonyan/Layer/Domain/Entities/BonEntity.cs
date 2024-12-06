using Bonyan.Layer.Domain.Entity;

namespace Bonyan.Layer.Domain.Entities;

/// <summary>
///     Base class for all entities in the domain, providing a method to retrieve the entity's keys.
/// </summary>
public abstract class BonEntity : IBonEntity, IEquatable<BonEntity>
{
    protected BonEntity()
    {
        BonEntityHelper.TrySetTenantId(this);
    }

    /// <summary>
    ///     Gets the array of keys that uniquely identify the entity.
    /// </summary>
    /// <returns>An object representing the entity's key(s).</returns>
    public abstract object GetKey();

    /// <summary>
    ///     Determines whether the current entity is equal to another entity.
    /// </summary>
    /// <param name="other">The other entity to compare with.</param>
    /// <returns>True if the entities are equal; otherwise, false.</returns>
    public bool Equals(BonEntity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        // Compare entities by their type and key(s)
        return GetType() == other.GetType() &&
               Equals(GetKey(), other.GetKey());
    }

    public override bool Equals(object? obj)
    {
        if (obj is BonEntity other)
            return Equals(other);

        return false;
    }

    public override int GetHashCode()
    {
        // Use the hash code of the key(s) for consistent hashing
        var key = GetKey();
        return key != null ? key.GetHashCode() : 0;
    }

    public static bool operator ==(BonEntity? left, BonEntity? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator !=(BonEntity? left, BonEntity? right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Provides a string representation of the entity's key(s) for debugging.
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name} [Key={GetKey()}]";
    }
}

/// <summary>
///     Base class for entities with a strongly-typed key.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public abstract class BonEntity<TKey> : BonEntity, IBonEntity<TKey>
{
    /// <summary>
    ///     Gets or sets the identifier for the entity.
    /// </summary>
    public TKey Id { get; set; } = default!; // Ensure non-nullability

    /// <summary>
    ///     Gets the array of keys that uniquely identify the entity.
    /// </summary>
    /// <returns>An array containing the entity's key.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the Id is null or default.</exception>
    public override object GetKey()
    {
        if (Id is null || Id.Equals(default(TKey)))
            throw new InvalidOperationException($"{nameof(Id)} cannot be null or default.");

        return Id;
    }
}