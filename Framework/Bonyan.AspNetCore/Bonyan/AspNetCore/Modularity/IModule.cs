namespace Bonyan.AspNetCore.Modularity;

public interface IModule
{
  List<IModule> NestedModule { get; set; }
  void AddModule<TModule>(Action<TModule>? action = null) where TModule : IModule;
  void OnPreConfigure(IBonyanApplicationBuilder builder);
  void OnConfigure(IBonyanApplicationBuilder builder);
  void OnPostConfigure(IBonyanApplicationBuilder builder);
  void OnBuild(BonyanApplication application);
  void OnPreBuild(BonyanApplication application);
  void OnPostBuild(BonyanApplication application);
}
