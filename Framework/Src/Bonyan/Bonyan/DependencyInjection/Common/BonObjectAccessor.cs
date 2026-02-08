namespace Bonyan.DependencyInjection;

/// <summary>
/// Default implementation of <see cref="IBonObjectAccessor{T}"/>; holds a mutable reference.
/// </summary>
public class BonObjectAccessor<T> : IBonObjectAccessor<T>
{
    /// <inheritdoc />
    public T? Value { get; set; }

    public BonObjectAccessor()
    {
    }

    public BonObjectAccessor(T? obj)
    {
        Value = obj;
    }
}