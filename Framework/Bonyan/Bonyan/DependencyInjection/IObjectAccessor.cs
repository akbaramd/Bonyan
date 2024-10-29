namespace Bonyan.DependencyInjection;

public interface IObjectAccessor<out T>
{
  T? Value { get; }
}
