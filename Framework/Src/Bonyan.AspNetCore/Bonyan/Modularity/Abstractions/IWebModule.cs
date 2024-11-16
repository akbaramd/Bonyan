namespace Bonyan.Modularity.Abstractions;

public interface IWebModule : IBonModule{
  
  Task OnPreApplicationAsync(BonWebApplicationContext webApplicationContext) ;
  
  Task OnApplicationAsync(BonWebApplicationContext webApplicationContext) ;
  
  Task OnPostApplicationAsync(BonWebApplicationContext webApplicationContext) ;
  
}
