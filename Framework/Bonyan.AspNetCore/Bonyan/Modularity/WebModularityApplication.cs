using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

public class WebModularityApplication<TModule> : ModularityApplication<TModule> , IWebModularityApplication where TModule : IModule
{
  public WebModularityApplication(IServiceCollection serviceCollection) : base(serviceCollection)
  {
  }

  public async Task InitializeApplicationAsync(WebApplication application)
  {
    var webModules = Modules.Select(x => x.Instance).OfType<IWebModule>().ToList();
    var ctx = new ApplicationContext(application);
    
    foreach (var webModule in webModules)
    {
     await webModule.OnPreApplicationAsync(ctx);
    }
            
    foreach (var webModule in webModules)
    {
      await webModule.OnApplicationAsync(ctx);
    }
            
    foreach (var webModule in webModules)
    {
      await webModule.OnPostApplicationAsync(ctx);
    }
  }
}
