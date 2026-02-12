using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// This class is equivalent of the <see cref="TransientCachedServiceProvider"/>.
/// Use <see cref="TransientCachedServiceProvider"/> instead of this class, for new projects. 
/// </summary>
public class BonLazyServiceProvider :
  CachedServiceProviderBase,
  IBonLazyServiceProvider
{
  private const string AgentLogPath = @"c:\Users\ahmadi.UR-NEZAM\RiderProjects\Bonyan\.cursor\debug.log";

  public BonLazyServiceProvider(IServiceProvider serviceProvider)
    : base(serviceProvider)
  {
#region agent log
    try
    {
      try
      {
        Console.Error.WriteLine(
          $"[agent-log] BonLazyServiceProvider.ctor reached | asm={typeof(BonLazyServiceProvider).Assembly.Location} | spType={serviceProvider?.GetType().FullName}");
      }
      catch
      {
        // ignore
      }

      var dir = Path.GetDirectoryName(AgentLogPath);
      if (!string.IsNullOrWhiteSpace(dir))
      {
        Directory.CreateDirectory(dir);
      }
      File.AppendAllText(
        AgentLogPath,
        JsonSerializer.Serialize(new
        {
          id = $"log_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid():N}",
          timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
          runId = "pre-fix",
          hypothesisId = "H3",
          location = "BonLazyServiceProvider.cs:ctor",
          message = "BonLazyServiceProvider received IServiceProvider",
          data = new
          {
            serviceProviderType = serviceProvider?.GetType().FullName,
            isAutofacServiceProvider = serviceProvider?.GetType().FullName == "Autofac.Extensions.DependencyInjection.AutofacServiceProvider"
          }
        }) + Environment.NewLine);
    }
    catch (Exception ex)
    {
      try
      {
        Console.Error.WriteLine($"[agent-log-write-failed] BonLazyServiceProvider.cs:ctor :: {ex.GetType().FullName} :: {ex.Message}");
      }
      catch
      {
        // ignore
      }
    }
#endregion
  }
    
  public virtual T LazyGetRequiredService<T>()
  {
    return (T)LazyGetRequiredService(typeof(T));
  }

  public virtual object LazyGetRequiredService(Type serviceType)
  {
    return this.GetRequiredService(serviceType);
  }

  public virtual T? LazyGetService<T>()
  {
    return (T?)LazyGetService(typeof(T));
  }

  public virtual object? LazyGetService(Type serviceType)
  {
    return GetService(serviceType);
  }

  public virtual T LazyGetService<T>(T defaultValue)
  {
    return GetService(defaultValue);
  }

  public virtual object LazyGetService(Type serviceType, object defaultValue)
  {
    return GetService(serviceType, defaultValue);
  }

  public virtual T LazyGetService<T>(Func<IServiceProvider, object> factory)
  {
    return GetService<T>(factory);
  }

  public virtual object LazyGetService(Type serviceType, Func<IServiceProvider, object> factory)
  {
    return GetService(serviceType, factory);
  }
}
