namespace Bonyan.Modularity.Abstractions;

public interface IWebModule : IBonModule{
  
  Task OnPreApplicationAsync(BonWebApplicationContext context) ;
  
  Task OnApplicationAsync(BonWebApplicationContext context) ;
  
  Task OnPostApplicationAsync(BonWebApplicationContext context) ;
  
}
