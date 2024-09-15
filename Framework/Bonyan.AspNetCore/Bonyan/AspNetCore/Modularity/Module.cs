namespace Bonyan.AspNetCore.Modularity;

public abstract class Module : IModule
{
  public List<IModule> NestedModule { get; set; } = new();

  public void AddModule<TModule>(Action<TModule>? action = null) where TModule : IModule
  {
    var module = Activator.CreateInstance<TModule>();
    NestedModule.Add(module);
    action?.Invoke(module);
  }

  public virtual void OnPreConfigure(IBonyanApplicationBuilder builder)
  {
    
  }

  public  virtual void OnConfigure(IBonyanApplicationBuilder builder)
  {
  }

  public virtual void OnPostConfigure(IBonyanApplicationBuilder builder)
  {
  }

  public virtual  void OnBuild(BonyanApplication application)
  {
  }

  public virtual void OnPreBuild(BonyanApplication application)
  {
  }

  public virtual void OnPostBuild(BonyanApplication application)
  {
  }
}
