using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

public interface IWebModule : IModule{
  
  Task OnPreApplicationAsync(ModularityApplicationContext app) ;
  
  Task OnApplicationAsync(ModularityApplicationContext app) ;
  
  Task OnPostApplicationAsync(ModularityApplicationContext app) ;
  
}
