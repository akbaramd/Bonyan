namespace Bonyan.Modularity.Abstractions;

public interface IWebModule : IModule{
  
  Task OnPreApplicationAsync(ApplicationContext context) ;
  
  Task OnApplicationAsync(ApplicationContext context) ;
  
  Task OnPostApplicationAsync(ApplicationContext context) ;
  
}
