namespace Bonyan.DependencyInjection;

public class BonObjectAccessor<T> : IBonObjectAccessor<T>
{
    public T? Value { get; set; }

    public BonObjectAccessor()
    {
    }
    public BonObjectAccessor(T? obj)
    {
        Value = obj;
    }
}