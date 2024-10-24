using Microsoft.Extensions.Configuration;

namespace Bonyan.Modularity;

public class ModularityApplicationContext
{
  public ModularityApplicationContext(BonyanApplication bonyanApplication)
  {
    BonyanApplication = bonyanApplication;
  }

  public BonyanApplication BonyanApplication { get; set; }

  public T? GetService<T>()
  {
    var serviceProvider = BonyanApplication.Application.Services;
    return serviceProvider.GetService<T>();
  }

  /// <summary>
  /// دریافت سرویس اجباری از طریق یک ServiceProvider موقت.
  /// </summary>
  public T RequireService<T>() where T : notnull
  {
    var serviceProvider = BonyanApplication.Application.Services;
    return serviceProvider.GetRequiredService<T>();
  }
}
