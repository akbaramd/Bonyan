using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

public class WebModularityApplication<TModule> : ModularityApplication<TModule> , IWebModularityApplication where TModule : IModule
{
  public WebModularityApplication(IServiceCollection serviceCollection) : base(serviceCollection)
  {
  }

  public async Task ApplicationAsync(ModularityApplicationContext bonyanApplication)
  {
    var webModules = Modules.Select(x => x.Instance).OfType<IWebModule>().ToList();
    foreach (var webModule in webModules)
    {
     await webModule.OnPreApplicationAsync(bonyanApplication);
    }
            
    foreach (var webModule in webModules)
    {
      await webModule.OnApplicationAsync(bonyanApplication);
    }
            
    foreach (var webModule in webModules)
    {
      await webModule.OnPostApplicationAsync(bonyanApplication);
    }
  }
}
