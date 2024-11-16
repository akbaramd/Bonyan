namespace Bonyan.Modularity.Abstractions;

public interface IBonModuleAccessor
{
    void ClearModules();
    void AddModule(ModuleInfo moduleInfo);
    ModuleInfo? GetModule(Type moduleType);
    IEnumerable<ModuleInfo> GetAllModules();
}