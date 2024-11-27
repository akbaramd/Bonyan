using Bonyan.DependencyInjection;
using Bonyan.Exceptions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionObjectAccessorExtensions
{
  public static BonObjectAccessor<T> TryAddObjectAccessor<T>(this IServiceCollection services)
  {
    if (services.Any(s => s.ServiceType == typeof(BonObjectAccessor<T>)))
    {
      return services.GetSingletonInstance<BonObjectAccessor<T>>();
    }

    return services.AddObjectAccessor<T>();
  }

  public static BonObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services)
  {
    return services.AddObjectAccessor(new BonObjectAccessor<T>());
  }

  public static BonObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services, T obj)
  {
    return services.AddObjectAccessor(new BonObjectAccessor<T>(obj));
  }

  public static BonObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services, BonObjectAccessor<T> accessor)
  {
    if (services.Any(s => s.ServiceType == typeof(BonObjectAccessor<T>)))
    {
      throw new BonException("An object accessor is registered before for type: " + typeof(T).AssemblyQualifiedName);
    }

    //Add to the beginning for fast retrieve
    services.Insert(0, ServiceDescriptor.Singleton(typeof(BonObjectAccessor<T>), accessor));
    services.Insert(0, ServiceDescriptor.Singleton(typeof(IBonObjectAccessor<T>), accessor));

    return accessor;
  }

  public static T? GetObjectOrNull<T>(this IServiceCollection services)
    where T : class
  {
    return services.GetSingletonInstanceOrNull<IBonObjectAccessor<T>>()?.Value;
  }

  public static T GetObject<T>(this IServiceCollection services)
    where T : class
  {
    return services.GetObjectOrNull<T>() ?? throw new BonException($"Could not find an object of {typeof(T).AssemblyQualifiedName} in services. Be sure that you have used AddObjectAccessor before!");
  }
}
