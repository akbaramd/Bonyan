using System;
using System.Reflection;
using System.Threading.Tasks;
using Bonyan.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.Modularity;

public class ModularityContext
{
  public IServiceCollection Services { get; set; }

  public IConfiguration Configuration { get; set; }

  public ModularityContext(IServiceCollection services, IConfiguration configuration)
  {
    Services = services;
    Configuration = configuration;
  }


  /// <summary>
  /// دریافت سرویس از طریق یک ServiceProvider موقت.
  /// </summary>
  public T? GetService<T>() where T : class
  {
    var serviceProvider = Services.BuildServiceProvider().CreateScope().ServiceProvider;
    return serviceProvider.GetService<T>();
  }

  /// <summary>
  /// دریافت سرویس اجباری از طریق یک ServiceProvider موقت.
  /// </summary>
  public T RequiredOption<T>() where T : class
  {

    var service = RequireService<IOptions<T>>();

      if (service.Value == null)
      {
        throw new ConfigurationNotFoundException<T>();
      }
      return service.Value;
  }
  public T? GetOption<T>() where T : class
  {

    var service = GetService<T>();

    if (service is IOptions<T> options)
    {
      return options.Value;
    }

    return service;
  }

  public T RequireService<T>() where T : class
  {
    var serviceProvider = Services.BuildServiceProvider().CreateScope().ServiceProvider;
    var service = serviceProvider.GetRequiredService<T>();
    return service;
  }
  
  public void Configure<TOptions>(Action<TOptions> configureOptions)
    where TOptions : class
  {
    Services.Configure(configureOptions);
  }

  public void Configure<TOptions>(string name, Action<TOptions> configureOptions)
    where TOptions : class
  {
    Services.Configure(name, configureOptions);
  }




}
