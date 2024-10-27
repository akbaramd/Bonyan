namespace Bonyan.FastEndpoints;

public class FastEndpointConfiguration
{
 
  private List<Action<WebApplication>> _beforeInitializer = new List<Action<WebApplication>>();
  private List<Action<WebApplication>> _afterInitializer = new List<Action<WebApplication>>();


  public void AddBeforeInitializer(Action<WebApplication> action)
  {
    _beforeInitializer.Add(action);
  }
  
  public void AddAfterInitializer(Action<WebApplication> action)
  {
    _afterInitializer.Add(action);
  }
  
  public void InitBefore(WebApplication bonyanApplication)
  {
    foreach (var action in _beforeInitializer)
    {
      action.Invoke(bonyanApplication);
    }
  }
  
  public void InitAfter(WebApplication bonyanApplication)
  {
    foreach (var action in _afterInitializer)
    {
      action.Invoke(bonyanApplication);
    }
  }

}
