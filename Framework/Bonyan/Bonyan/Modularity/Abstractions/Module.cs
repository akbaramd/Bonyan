namespace Bonyan.Modularity.Abstractions;

public abstract class Module:IModule
{
  public virtual Task OnPreConfigureAsync(ModularityContext context) {
    return Task.CompletedTask;
  }
  public virtual Task OnConfigureAsync(ModularityContext context) {
    return Task.CompletedTask;
  }
  public virtual Task OnPostConfigureAsync(ModularityContext context) {
    return Task.CompletedTask;
  }
  
  public virtual Task OnPreInitializeAsync(ModularityInitializedContext context) {
    return Task.CompletedTask;
  }
  public virtual Task OnInitializeAsync(ModularityInitializedContext context) {
    return Task.CompletedTask;
  }
  public virtual Task OnPostInitializeAsync(ModularityInitializedContext context) {
    return Task.CompletedTask;
  }
}
