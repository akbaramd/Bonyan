namespace Bonyan.DependencyInjection;

/// <summary>
/// Holds a reference to an object (e.g. for ambient or scoped access).
/// </summary>
public interface IBonObjectAccessor<out T>
{
    /// <summary>Current value, or null if not set.</summary>
    T? Value { get; }
}
