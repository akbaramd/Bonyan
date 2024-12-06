using System.ComponentModel;

namespace Bonyan.Layer.Domain.ValueObjects;

public abstract class BonBusinessId<T, TKey> : BonValueObject, IEquatable<BonBusinessId<T, TKey>>
    where T : BonBusinessId<T, TKey>, new()
{
    protected BonBusinessId()
    {
    }

    protected BonBusinessId(TKey value)
    {
        if (value == null || value.Equals(default(TKey)))
            throw new ArgumentException($"The value of {nameof(value)} cannot be null or default.", nameof(value));

        Value = value;
    }

    public TKey Value { get; private set; }

    /// <summary>
    ///     Determines whether this instance and another specified instance have the same value.
    /// </summary>
    public bool Equals(BonBusinessId<T, TKey>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return EqualityComparer<TKey>.Default.Equals(Value, other.Value);
    }

    /// <summary>
    ///     Factory method to create an instance of the derived class from an existing value.
    /// </summary>
    public static T FromValue(TKey value)
    {
        if (value == null || value.Equals(default(TKey)))
            throw new ArgumentException($"The value of {nameof(value)} cannot be null or default.", nameof(value));

        return NewId(value);
    }

    /// <summary>
    ///     Helper method to create an instance of the derived class.
    /// </summary>
    public static T NewId(TKey value)
    {
        var instance = new T();
        instance.Value = value;
        return instance;
    }

    public override bool Equals(object? obj)
    {
        if (obj is BonBusinessId<T, TKey> other)
            return Equals(other);

        return false;
    }

    public override int GetHashCode()
    {
        return Value?.GetHashCode() ?? 0;
    }

    // Overrides equality operators for value comparison
    public static bool operator ==(BonBusinessId<T, TKey>? left, BonBusinessId<T, TKey>? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator !=(BonBusinessId<T, TKey>? left, BonBusinessId<T, TKey>? right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }

    // Overrides equality components for comparing value objects
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}

/// <summary>
///     Represents a specialized implementation of BonBusinessId with a GUID as the key type.
/// </summary>
public abstract class BonBusinessId<T> : BonBusinessId<T, Guid> where T : BonBusinessId<T>, new()
{
    public BonBusinessId() : base(Guid.NewGuid())
    {
        // Default constructor prevents direct instantiation without valid GUID
    }

    public BonBusinessId(Guid value) : base(value)
    {
    }

    /// <summary>
    ///     Factory method to create a new BusinessId with a new GUID.
    /// </summary>
    public static T NewId()
    {
        return FromValue(Guid.NewGuid());
    }

    /// <summary>
    ///     Factory method to create a BusinessId from a string representation of a GUID.
    /// </summary>
    public static T FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Business ID cannot be empty or whitespace.", nameof(value));

        if (!Guid.TryParse(value, out var parsedGuid))
            throw new ArgumentException("Invalid GUID format.", nameof(value));

        return FromValue(parsedGuid);
    }
}