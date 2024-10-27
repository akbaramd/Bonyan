

namespace Bonyan.AspNetCore.Context;

public class BonyanContext : IBonyanContext
{
  private readonly BonyanApplicationBuilder _bonyanApplicationBuilder;

  

  private readonly List<Action<WebApplication>> _syncActions = new();
  private readonly List<Action<WebApplication>> _syncBeforeActions = new();
  
  public BonyanContext(BonyanApplicationBuilder bonyanApplicationBuilder)
  {
    _bonyanApplicationBuilder = bonyanApplicationBuilder;
  }
  
  
  // Method for adding cron jobs
 

  public void AddScoped<TService, TImplementation>() where TService : class where TImplementation : class, TService
  {
    _bonyanApplicationBuilder.Services.AddScoped<TService, TImplementation>();
  }

  public void AddScoped<TService>() where TService : class
  {
    _bonyanApplicationBuilder.Services.AddScoped<TService>();
  }

  public void AddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService
  {
    _bonyanApplicationBuilder.Services.AddSingleton<TService, TImplementation>();
  }

  public void AddSingleton<TService>() where TService : class
  {
    _bonyanApplicationBuilder.Services.AddSingleton<TService>();
  }

  public void AddTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService
  {
    _bonyanApplicationBuilder.Services.AddTransient<TService, TImplementation>();
  }

  public void AddTransient<TService>() where TService : class
  {
    _bonyanApplicationBuilder.Services.AddTransient<TService>();
  }


  // Implement AddScoped(Type serviceType, Type implementationType)
  public void AddScoped(Type serviceType, Type implementationType)
  {
    _bonyanApplicationBuilder.Services.AddScoped(serviceType, implementationType);
  }

  public void AddScoped(Type serviceType)
  {
    _bonyanApplicationBuilder.Services.AddScoped(serviceType);
  }

  public void AddSingleton(Type serviceType, Type implementationType)
  {
    _bonyanApplicationBuilder.Services.AddSingleton(serviceType, implementationType);
  }

  public void AddSingleton(Type serviceType)
  {
    _bonyanApplicationBuilder.Services.AddSingleton(serviceType);
  }

  public void AddTransient(Type serviceType, Type implementationType)
  {
    _bonyanApplicationBuilder.Services.AddTransient(serviceType, implementationType);
  }

  public void AddTransient(Type serviceType)
  {
    _bonyanApplicationBuilder.Services.AddTransient(serviceType);
  }

  public void AddBeforeInitializer(Action<WebApplication> action)
          {
            _syncBeforeActions.Add(action);
          }
  
          public void AddInitializer(Action<WebApplication> action)
          {
              _syncActions.Add(action);
          }
  
          public void AddBeforeInitializer<TInitializer>() where TInitializer : class, IWebApplicationInitializer
          {
            _bonyanApplicationBuilder.Services.AddSingleton<TInitializer>();
            _syncBeforeActions.Add(app =>
            {
              try
              {
                var initializer = app.Services.GetRequiredService<TInitializer>();
                initializer.InitializeAsync(app).GetAwaiter().GetResult();
              }
              catch (Exception ex)
              {
                Console.WriteLine($"Failed to initialize {typeof(TInitializer).Name}: {ex.Message}");
              }
            });
          }
  
          public void AddInitializer<TInitializer>() where TInitializer : class, IWebApplicationInitializer
          {
            _bonyanApplicationBuilder.Services.AddSingleton<TInitializer>();
              _syncActions.Add(app =>
              {
                  try
                  {
                      var initializer = app.Services.GetRequiredService<TInitializer>();
                      initializer.InitializeAsync(app).GetAwaiter().GetResult();
                  }
                  catch (Exception ex)
                  {
                      Console.WriteLine($"Failed to initialize {typeof(TInitializer).Name}: {ex.Message}");
                  }
              });
          }

  public void Build(WebApplication application)
  {
    foreach (var action in _syncBeforeActions)
    {
      action.Invoke(application);
    }
    foreach (var action in _syncActions)
    {
      action.Invoke(application);
    }

 
  }
  
  
  
}
