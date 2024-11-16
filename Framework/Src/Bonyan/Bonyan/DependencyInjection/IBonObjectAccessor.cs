namespace Bonyan.DependencyInjection;

public interface IBonObjectAccessor<out T>
{
  T? Value { get; }
}
