namespace Bonyan.Modularity.Abstractions;

public interface IModuleLoader
{
    void LoadModules(Type mainModuleType);
    IEnumerable<ModuleInfo> GetLoadedModules();
}
