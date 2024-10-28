using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

public class ModuleInfo
{
    public Type ModuleType { get; }
    public IModule? Instance { get; set; }
    public List<ModuleInfo> Dependencies { get; }
    public bool IsLoaded { get; set; }

    public ModuleInfo(Type moduleType)
    {
        ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
        Dependencies = new List<ModuleInfo>();
        IsLoaded = false;
    }
}