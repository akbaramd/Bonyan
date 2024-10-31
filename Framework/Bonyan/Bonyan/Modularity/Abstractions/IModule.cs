namespace Bonyan.Modularity.Abstractions;

public interface IModule : IConfigurableModule, IInitializableModule
{
    public List<Type> DependedModules { get; set; }


    void DependOn<TModule>() where TModule : IModule;
    void DependOn(params Type[] type);
}
