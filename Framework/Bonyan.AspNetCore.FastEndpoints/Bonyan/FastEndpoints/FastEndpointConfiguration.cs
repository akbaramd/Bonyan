namespace Bonyan.FastEndpoints;

public class FastEndpointConfiguration
{
 
  private List<Action<BonyanApplication>> _beforeInitializer = new List<Action<BonyanApplication>>();
  private List<Action<BonyanApplication>> _afterInitializer = new List<Action<BonyanApplication>>();


  public void AddBeforeInitializer(Action<BonyanApplication> action)
  {
    _beforeInitializer.Add(action);
  }
  
  public void AddAfterInitializer(Action<BonyanApplication> action)
  {
    _afterInitializer.Add(action);
  }
  
  public void InitBefore(BonyanApplication bonyanApplication)
  {
    foreach (var action in _beforeInitializer)
    {
      action.Invoke(bonyanApplication);
    }
  }
  
  public void InitAfter(BonyanApplication bonyanApplication)
  {
    foreach (var action in _afterInitializer)
    {
      action.Invoke(bonyanApplication);
    }
  }

}
