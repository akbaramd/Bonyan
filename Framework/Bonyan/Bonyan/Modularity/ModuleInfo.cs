using System.Reflection;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

public class ModuleInfo
{
    public Type ModuleType { get; }
    public IBonModule? Instance { get; set; }
    public List<ModuleInfo> Dependencies { get; }
    
    Assembly Assembly { get; }
    
    /// <summary>
    /// All the assemblies of the module.
    /// Includes the main <see cref="Assembly"/> and other assemblies defined
    /// on the module <see cref="Type"/> using the <see cref="AdditionalAssemblyAttribute"/> attribute.
    /// </summary>
    public Assembly[] AllAssemblies { get; }
    public bool IsLoaded { get; set; }

    public ModuleInfo(Type moduleType)
    {
        ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
        Dependencies = new List<ModuleInfo>();
        Assembly = moduleType.Assembly;
        AllAssemblies = BonyanModuleHelper.GetAllAssemblies(moduleType);
        IsLoaded = false;
    }
}