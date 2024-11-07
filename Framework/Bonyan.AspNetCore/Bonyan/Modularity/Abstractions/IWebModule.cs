namespace Bonyan.Modularity.Abstractions;

public interface IWebModule : IBonModule{
  
  Task OnPreApplicationAsync(BonContext context) ;
  
  Task OnApplicationAsync(BonContext context) ;
  
  Task OnPostApplicationAsync(BonContext context) ;
  
}
