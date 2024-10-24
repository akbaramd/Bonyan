using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity;

public class ModularityInitializedContext
{
  public IServiceProvider Services { get; set; }

  public IConfiguration Configuration { get; set; }

  public ModularityInitializedContext(IServiceProvider services, IConfiguration configuration)
  {
    Services = services;
    Configuration = configuration;
  }
  
  
  public T? GetService<T>()
  {
  
    return Services.GetService<T>();
  }

  /// <summary>
  /// دریافت سرویس اجباری از طریق یک ServiceProvider موقت.
  /// </summary>
  public T RequireService<T>() where T : notnull
  {
   
    return Services.GetRequiredService<T>();
  }
}
