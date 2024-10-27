using Microsoft.Extensions.Configuration;

namespace Bonyan.Modularity;

public class ModularityApplicationContext
{
  public ModularityApplicationContext(WebApplication webApplication)
  {
    WebApplication = webApplication;
  }

  public WebApplication WebApplication { get; set; }

  public T? GetService<T>()
  {
    var serviceProvider = WebApplication.Services;
    return serviceProvider.GetService<T>();
  }

  /// <summary>
  /// دریافت سرویس اجباری از طریق یک ServiceProvider موقت.
  /// </summary>
  public T RequireService<T>() where T : notnull
  {
    var serviceProvider = WebApplication.Services;
    return serviceProvider.GetRequiredService<T>();
  }
}
