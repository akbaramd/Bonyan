

namespace Bonyan.AspNetCore.Context;

public interface IBonyanContext
{
  // Synchronous methods
  void AddBeforeInitializer(Action<WebApplication> action);
  void AddInitializer(Action<WebApplication> action);
  void AddBeforeInitializer<TInitializer>() where TInitializer : class, IWebApplicationInitializer;
  void AddInitializer<TInitializer>() where TInitializer : class, IWebApplicationInitializer;


  // Dependency Injection methods
  void AddScoped<TService, TImplementation>() where TService : class where TImplementation : class, TService;
  void AddScoped<TService>() where TService : class;
  void AddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService;
  void AddSingleton<TService>() where TService : class;
  void AddTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService;
  void AddTransient<TService>() where TService : class;
  
  // Dependency Injection methods (type versions)
  void AddScoped(Type serviceType, Type implementationType);
  void AddScoped(Type serviceType);
  void AddSingleton(Type serviceType, Type implementationType);
  void AddSingleton(Type serviceType);
  void AddTransient(Type serviceType, Type implementationType);
  void AddTransient(Type serviceType);
  
  void Build(WebApplication application);
}
