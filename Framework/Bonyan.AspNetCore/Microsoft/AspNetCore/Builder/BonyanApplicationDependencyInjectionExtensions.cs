namespace Microsoft.AspNetCore.Builder;

public  static class BonyanApplicationDependencyInjectionExtensions
{
  public static IBonyanApplicationBuilder RegisterService(this IBonyanApplicationBuilder applicationBuilder ,Type service,Type implement, ServiceLifetime lifetime = ServiceLifetime.Scoped) 
  {
    switch (lifetime)
    {
      case ServiceLifetime.Singleton:
        applicationBuilder.Services.AddSingleton(service,implement);
        break;
      case ServiceLifetime.Scoped:
        applicationBuilder.Services.AddScoped(service,implement);
        break;
      case ServiceLifetime.Transient:
        applicationBuilder.Services.AddTransient(service,implement);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
    }
    return applicationBuilder;
  }
  public static IBonyanApplicationBuilder RegisterService(this IBonyanApplicationBuilder applicationBuilder ,Type implement, ServiceLifetime lifetime = ServiceLifetime.Scoped) 
  {
    switch (lifetime)
    {
      case ServiceLifetime.Singleton:
        applicationBuilder.Services.AddSingleton(implement);
        break;
      case ServiceLifetime.Scoped:
        applicationBuilder.Services.AddScoped(implement);
        break;
      case ServiceLifetime.Transient:
        applicationBuilder.Services.AddTransient(implement);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
    }
    return applicationBuilder;
  }
  
  public static IBonyanApplicationBuilder RegisterService<TImplement>(this IBonyanApplicationBuilder applicationBuilder,Func<IServiceProvider, TImplement> ac, ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplement : class
  {
    switch (lifetime)
    {
      case ServiceLifetime.Singleton:
        applicationBuilder.Services.AddSingleton(ac);
        break;
      case ServiceLifetime.Scoped:
        applicationBuilder.Services.AddScoped(ac);
        break;
      case ServiceLifetime.Transient:
        applicationBuilder.Services.AddTransient(ac);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
    }
    return applicationBuilder;
  }
  
  public static IBonyanApplicationBuilder RegisterService<TType,TImplement>(this IBonyanApplicationBuilder applicationBuilder , ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplement : class, TType where TType : class
  {
    
    return RegisterService(applicationBuilder,typeof(TType),typeof(TImplement),lifetime);
  }
  
  public static IBonyanApplicationBuilder RegisterService<TImplement>(this IBonyanApplicationBuilder applicationBuilder, ServiceLifetime lifetime = ServiceLifetime.Scoped) where TImplement : class
  {
    return RegisterService(applicationBuilder,typeof(TImplement),lifetime);
  }
  

  public static IBonyanApplicationBuilder Configure<TType>(this IBonyanApplicationBuilder applicationBuilder,Action<TType> action , string? name = null) where TType : class
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      
      applicationBuilder.Services.Configure(action);
    }
    else
    {
      applicationBuilder.Services.Configure(name,action);
    }

    return applicationBuilder;
  }
  
  
  public static IBonyanApplicationBuilder AddHostedService<TType>(this IBonyanApplicationBuilder applicationBuilder) where TType : class, IHostedService
  {
    applicationBuilder.Services.AddHostedService<TType>();
    return applicationBuilder;
  }
  
  
  public static IBonyanApplicationBuilder AddHostedService<TType>(this IBonyanApplicationBuilder applicationBuilder,Func<IServiceProvider, TType> ac) where TType : class, IHostedService
  {
    applicationBuilder.Services.AddHostedService(ac);
    return applicationBuilder;
  }
  
}
