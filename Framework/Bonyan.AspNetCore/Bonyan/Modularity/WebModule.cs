using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

public abstract class WebModule:Module,IWebModule
{
  public virtual Task OnPreApplicationAsync(ModularityApplicationContext app) {
    return Task.CompletedTask;
  }
  public virtual Task OnApplicationAsync(ModularityApplicationContext app) {
    return Task.CompletedTask;
  }
  public virtual Task OnPostApplicationAsync(ModularityApplicationContext app) {
    return Task.CompletedTask;
  }

}
