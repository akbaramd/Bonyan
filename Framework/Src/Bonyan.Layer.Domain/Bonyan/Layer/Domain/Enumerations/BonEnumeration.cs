using System.Reflection;
using Bonyan.Core;
using Bonyan.Layer.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bonyan.Layer.Domain.Enumerations;

/// <summary>
///     Base class for implementing strongly-typed enums with additional utility methods.
/// </summary>
public abstract class BonEnumeration : BonValueObject, IComparable
{
    

    /// <summary>
    ///     Initializes a new instance of the <see cref="BonEnumeration" /> class.
    /// </summary>
    /// <param name="id">The identifier of the enumeration.</param>
    /// <param name="name">The name of the enumeration.</param>
    protected BonEnumeration(int id, string name)
    {
        Name = Check.NotNull(name, nameof(name));
        Id = id;
    }

    /// <summary>
    ///     Gets the name of the enumeration.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Gets the identifier of the enumeration.
    /// </summary>
    public int Id { get;private set; }

    /// <summary>
    ///     Compares this enumeration instance to another based on Id.
    /// </summary>
    /// <param name="other">The other enumeration instance to compare to.</param>
    /// <returns>An integer that indicates the relative order of the objects being compared.</returns>
    /// <exception cref="ArgumentException">Thrown when the other object is not of type <see cref="BonEnumeration" />.</exception>
    public int CompareTo(object? other)
    {
        if (other is not BonEnumeration otherEnumeration)
            throw new ArgumentException($"Object must be of type {nameof(BonEnumeration)}", nameof(other));

        return Id.CompareTo(otherEnumeration.Id);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Id;
    }

    /// <summary>
    ///     Returns a string representation of the enumeration.
    /// </summary>
    /// <returns>The name of the enumeration.</returns>
    public override string ToString()
    {
        return Name;
    }

    /// <summary>
    ///     Determines whether the specified object is equal to the current enumeration.
    /// </summary>
    /// <param name="obj">The object to compare with the current enumeration.</param>
    /// <returns>True if the specified object is equal to the current enumeration; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is BonEnumeration otherValue && GetType() == obj.GetType() && Id == otherValue.Id;
    }

    /// <summary>
    ///     Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    ///     Implicitly converts an enumeration to its integer Id.
    /// </summary>
    /// <param name="bonEnumeration">The enumeration to convert.</param>
    /// <returns>The Id of the enumeration.</returns>
    public static implicit operator int(BonEnumeration bonEnumeration)
    {
        return bonEnumeration.Id;
    }

    /// <summary>
    ///     Implicitly converts an enumeration to its string Name.
    /// </summary>
    /// <param name="bonEnumeration">The enumeration to convert.</param>
    /// <returns>The Name of the enumeration.</returns>
    public static implicit operator string(BonEnumeration bonEnumeration)
    {
        return bonEnumeration.Name;
    }

    /// <summary>
    ///     Retrieves all instances of the enumeration type.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <returns>An enumerable collection of all enumeration instances.</returns>
    public static IEnumerable<T> GetAll<T>() where T : BonEnumeration
    {
        return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();
    }

    /// <summary>
    ///     Retrieves an enumeration instance by its Id.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="id">The Id of the enumeration.</param>
    /// <returns>The enumeration instance if found; otherwise, null.</returns>
    public static T? FromId<T>(int id) where T : BonEnumeration
    {
        return GetAll<T>().FirstOrDefault(item => item.Id == id);
    }

    /// <summary>
    ///     Retrieves an enumeration instance by its Name.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="name">The Name of the enumeration.</param>
    /// <returns>The enumeration instance if found; otherwise, null.</returns>
    public static T? FromName<T>(string name) where T : BonEnumeration
    {
        Check.NotNullOrEmpty(name, nameof(name));
        return GetAll<T>().FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    ///     Tries to retrieve an enumeration instance by its Id.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="id">The Id of the enumeration.</param>
    /// <param name="result">The output enumeration instance if found; otherwise, null.</param>
    /// <returns>True if the enumeration instance is found; otherwise, false.</returns>
    public static bool TryParse<T>(int id, out T? result) where T : BonEnumeration
    {
        result = FromId<T>(id);
        return result != null;
    }

    /// <summary>
    ///     Tries to retrieve an enumeration instance by its Name.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="name">The Name of the enumeration.</param>
    /// <param name="result">The output enumeration instance if found; otherwise, null.</param>
    /// <returns>True if the enumeration instance is found; otherwise, false.</returns>
    public static bool TryParse<T>(string name, out T? result) where T : BonEnumeration
    {
        Check.NotNullOrEmpty(name, nameof(name));
        result = FromName<T>(name);
        return result != null;
    }

    /// <summary>
    ///     Equality operator for Enumeration.
    /// </summary>
    /// <param name="left">The left Enumeration to compare.</param>
    /// <param name="right">The right Enumeration to compare.</param>
    /// <returns>True if both enumerations are equal; otherwise, false.</returns>
    public static bool operator ==(BonEnumeration? left, BonEnumeration? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    ///     Inequality operator for Enumeration.
    /// </summary>
    /// <param name="left">The left Enumeration to compare.</param>
    /// <param name="right">The right Enumeration to compare.</param>
    /// <returns>True if both enumerations are not equal; otherwise, false.</returns>
    public static bool operator !=(BonEnumeration? left, BonEnumeration? right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Less than operator for Enumeration.
    /// </summary>
    /// <param name="left">The left Enumeration to compare.</param>
    /// <param name="right">The right Enumeration to compare.</param>
    /// <returns>True if the left enumeration is less than the right; otherwise, false.</returns>
    public static bool operator <(BonEnumeration left, BonEnumeration right)
    {
        Check.NotNull(left, nameof(left));
        Check.NotNull(right, nameof(right));
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    ///     Greater than operator for Enumeration.
    /// </summary>
    /// <param name="left">The left Enumeration to compare.</param>
    /// <param name="right">The right Enumeration to compare.</param>
    /// <returns>True if the left enumeration is greater than the right; otherwise, false.</returns>
    public static bool operator >(BonEnumeration left, BonEnumeration right)
    {
        Check.NotNull(left, nameof(left));
        Check.NotNull(right, nameof(right));
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    ///     Less than or equal operator for Enumeration.
    /// </summary>
    /// <param name="left">The left Enumeration to compare.</param>
    /// <param name="right">The right Enumeration to compare.</param>
    /// <returns>True if the left enumeration is less than or equal to the right; otherwise, false.</returns>
    public static bool operator <=(BonEnumeration left, BonEnumeration right)
    {
        Check.NotNull(left, nameof(left));
        Check.NotNull(right, nameof(right));
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    ///     Greater than or equal operator for Enumeration.
    /// </summary>
    /// <param name="left">The left Enumeration to compare.</param>
    /// <param name="right">The right Enumeration to compare.</param>
    /// <returns>True if the left enumeration is greater than or equal to the right; otherwise, false.</returns>
    public static bool operator >=(BonEnumeration left, BonEnumeration right)
    {
        Check.NotNull(left, nameof(left));
        Check.NotNull(right, nameof(right));
        return left.CompareTo(right) >= 0;
    }

    /// <summary>
    ///     Retrieves the names of all instances of the enumeration type.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <returns>An enumerable collection of all enumeration names.</returns>
    public static IEnumerable<string> GetNames<T>() where T : BonEnumeration
    {
        return GetAll<T>().Select(e => e.Name);
    }

    /// <summary>
    ///     Retrieves the Ids of all instances of the enumeration type.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <returns>An enumerable collection of all enumeration Ids.</returns>
    public static IEnumerable<int> GetIds<T>() where T : BonEnumeration
    {
        return GetAll<T>().Select(e => e.Id);
    }

    /// <summary>
    ///     Retrieves an enumeration instance by its Id, or returns null if not found.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="id">The Id of the enumeration.</param>
    /// <returns>The enumeration instance if found; otherwise, null.</returns>
    public static T? GetByIdOrDefault<T>(int id) where T : BonEnumeration
    {
        return FromId<T>(id);
    }

    /// <summary>
    ///     Retrieves an enumeration instance by its Id, or throws an exception if not found.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="id">The Id of the enumeration.</param>
    /// <returns>The enumeration instance if found.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no enumeration with the specified Id is found.</exception>
    public static T GetByIdOrThrow<T>(int id) where T : BonEnumeration
    {
        return FromId<T>(id) ?? throw new InvalidOperationException($"No {typeof(T).Name} with Id {id} found.");
    }

    /// <summary>
    ///     Retrieves an enumeration instance by its Name, or returns null if not found.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="name">The Name of the enumeration.</param>
    /// <returns>The enumeration instance if found; otherwise, null.</returns>
    public static T? GetByNameOrDefault<T>(string name) where T : BonEnumeration
    {
        Check.NotNullOrEmpty(name, nameof(name));
        return FromName<T>(name);
    }

    /// <summary>
    ///     Retrieves an enumeration instance by its Name, or throws an exception if not found.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="name">The Name of the enumeration.</param>
    /// <returns>The enumeration instance if found.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no enumeration with the specified Name is found.</exception>
    public static T GetByNameOrThrow<T>(string name) where T : BonEnumeration
    {
        Check.NotNullOrEmpty(name, nameof(name));
        return FromName<T>(name) ??
               throw new InvalidOperationException($"No {typeof(T).Name} with Name '{name}' found.");
    }

    /// <summary>
    ///     Retrieves a list of Id and Name pairs for all instances of the enumeration type.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <returns>An enumerable collection of Id and Name pairs.</returns>
    public static IEnumerable<(int Id, string Name)> GetIdNamePairs<T>() where T : BonEnumeration
    {
        return GetAll<T>().Select(e => (e.Id, e.Name));
    }

    /// <summary>
    ///     Provides a custom value comparer for Entity Framework Core when mapping Enumeration types.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <returns>A <see cref="ValueComparer{T}" /> for the enumeration type.</returns>
    public static ValueComparer<T> GetValueComparer<T>() where T : BonEnumeration
    {
        return new ValueComparer<T>(
            (l, r) => l == r || (l != null && l.Equals(r)),
            v => v.GetHashCode(),
            v => (T)Activator.CreateInstance(typeof(T), v.Id, v.Name)!
        );
    }
}