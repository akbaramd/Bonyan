using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

public abstract class CachedServiceProviderBase : IBonCachedServiceProviderBase
{
  protected IServiceProvider ServiceProvider { get; }
  protected ConcurrentDictionary<ServiceIdentifier, Lazy<object?>> CachedServices { get; }

  protected CachedServiceProviderBase(IServiceProvider serviceProvider)
  {
    ServiceProvider = serviceProvider;
    CachedServices = new ConcurrentDictionary<ServiceIdentifier, Lazy<object?>>();
    CachedServices.TryAdd(new ServiceIdentifier(typeof(IServiceProvider)), new Lazy<object?>(() => ServiceProvider));
  }

  public virtual object? GetService(Type serviceType)
  {
    return CachedServices.GetOrAdd(
        new ServiceIdentifier(serviceType),
        _ => new Lazy<object?>(() => ServiceProvider.GetService(serviceType))
    ).Value;
  }

  public T GetService<T>(T defaultValue)
  {
    return (T)GetService(typeof(T), defaultValue!);
  }

  public object GetService(Type serviceType, object defaultValue)
  {
    return GetService(serviceType) ?? defaultValue;
  }

  public T GetService<T>(Func<IServiceProvider, object> factory)
  {
    return (T)GetService(typeof(T), factory);
  }

  public object GetService(Type serviceType, Func<IServiceProvider, object> factory)
  {
    return CachedServices.GetOrAdd(
        new ServiceIdentifier(serviceType),
        _ => new Lazy<object?>(() => factory(ServiceProvider))
    ).Value!;
  }

  public object? GetKeyedService(Type serviceType, object? serviceKey)
  {
    return CachedServices.GetOrAdd(
        new ServiceIdentifier(serviceKey, serviceType),
        _ => new Lazy<object?>(() => ServiceProvider.GetRequiredKeyedService(serviceType, serviceKey))
    ).Value;
  }

  public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
  {
    return CachedServices.GetOrAdd(
        new ServiceIdentifier(serviceKey, serviceType),
        _ => new Lazy<object?>(() => ServiceProvider.GetRequiredKeyedService(serviceType, serviceKey))
    ).Value!;
  }
}

public readonly struct ServiceIdentifier : IEquatable<ServiceIdentifier>
{
  public object? ServiceKey { get; }

  public Type ServiceType { get; }

  public ServiceIdentifier(Type serviceType)
  {
    ServiceType = serviceType;
  }

  public ServiceIdentifier(object? serviceKey, Type serviceType)
  {
    ServiceKey = serviceKey;
    ServiceType = serviceType;
  }

  public bool Equals(ServiceIdentifier other)
  {
    if (ServiceKey == null && other.ServiceKey == null)
    {
      return ServiceType == other.ServiceType;
    }
    else if (ServiceKey != null && other.ServiceKey != null)
    {
      return ServiceType == other.ServiceType && ServiceKey.Equals(other.ServiceKey);
    }
    return false;
  }

  public override bool Equals(object? obj)
  {
    return obj is ServiceIdentifier && Equals((ServiceIdentifier)obj);
  }

  public override int GetHashCode()
  {
    if (ServiceKey == null)
    {
      return ServiceType.GetHashCode();
    }
    unchecked
    {
      return (ServiceType.GetHashCode() * 397) ^ ServiceKey.GetHashCode();
    }
  }
}