using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

public class BonModuleAccessor : IBonModuleAccessor
{
    private readonly Dictionary<Type, ModuleInfo> _modules = new();


    public BonModuleAccessor()
    {
        _modules.Add(typeof(BonMasterModule),new ModuleInfo(typeof(BonMasterModule)));
    }
    
    public void AddModule(ModuleInfo moduleInfo)
    {
        _modules[moduleInfo.ModuleType] = moduleInfo;
    }

    public void ClearModules()
    {
        _modules.Clear();
    }
    public ModuleInfo? GetModule(Type moduleType) =>
        _modules.TryGetValue(moduleType, out var moduleInfo) ? moduleInfo : null;

    public IEnumerable<ModuleInfo> GetAllModules() => _modules.Values.ToList().AsReadOnly();
}