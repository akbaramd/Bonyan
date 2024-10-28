using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bonyan.Layer.Domain.ValueObjects;

/// <summary>
/// Represents a base class for value objects in a domain-driven design context.
/// Enforces immutability and provides equality operations.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Determines whether the specified value object is equal to the current value object.
    /// </summary>
    /// <param name="other">The value object to compare with the current value object.</param>
    /// <returns>True if the specified value object is equal to the current value object; otherwise, false.</returns>
    public bool Equals(ValueObject? other)
    {
        if (other is null || other.GetType() != GetType())
        {
            return false;
        }

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current value object.
    /// </summary>
    /// <param name="obj">The object to compare with the current value object.</param>
    /// <returns>True if the specified object is equal to the current value object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        return Equals(obj as ValueObject);
    }

    /// <summary>
    /// Serves as the hash function for the value object.
    /// </summary>
    /// <returns>A hash code for the current value object.</returns>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, obj) =>
            {
                unchecked
                {
                    return (current * 23) + (obj?.GetHashCode() ?? 0);
                }
            });
    }

    /// <summary>
    /// Determines whether two value objects are equal.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>True if the value objects are equal; otherwise, false.</returns>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two value objects are not equal.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>True if the value objects are not equal; otherwise, false.</returns>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Returns a string that represents the current value object, displaying its properties.
    /// </summary>
    /// <returns>A string representation of the value object.</returns>
    public override string ToString()
    {
        var type = GetType();
        var properties = type.GetProperties();
        var values = properties.Select(p => $"{p.Name}: {p.GetValue(this) ?? "null"}");
        return $"{type.Name} ({string.Join(", ", values)})";
    }

    /// <summary>
    /// Creates a shallow copy of the current value object.
    /// </summary>
    /// <returns>A shallow copy of the current value object.</returns>
    public ValueObject Clone()
    {
        return (ValueObject)MemberwiseClone();
    }

    /// <summary>
    /// Provides a custom value comparer for Entity Framework Core when mapping value objects with collections.
    /// </summary>
    /// <typeparam name="T">The value object type.</typeparam>
    /// <returns>A value comparer for the specified value object type.</returns>
    public static ValueComparer<T> GetValueComparer<T>() where T : ValueObject
    {
        return new ValueComparer<T>(
            (l, r) => l == r,
            v => v.GetHashCode(),
            v => (T)v.Clone()
        );
    }

    /// <summary>
    /// Gets the components of the value object that are used for equality comparison.
    /// Must be overridden in derived classes to specify the properties that define equality.
    /// </summary>
    /// <returns>An enumeration of the components that define the value object's equality.</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();
}
