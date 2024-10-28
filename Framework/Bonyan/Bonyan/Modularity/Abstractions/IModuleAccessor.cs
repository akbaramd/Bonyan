namespace Bonyan.Modularity.Abstractions;

public interface IModuleAccessor
{
    void ClearModules();
    void AddModule(ModuleInfo moduleInfo);
    ModuleInfo? GetModule(Type moduleType);
    IEnumerable<ModuleInfo> GetAllModules();
}