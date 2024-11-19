using System.Reflection;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

public class BonModuleDescriptor
{
    public Type ModuleType { get; }
    public IBonModule? Instance { get; set; }
    public List<BonModuleDescriptor> Dependencies { get; }
    
    Assembly Assembly { get; }
    
    /// <summary>
    /// All the assemblies of the module.
    /// Includes the main <see cref="Assembly"/> and other assemblies defined
    /// on the module <see cref="Type"/> using the <see cref="AdditionalAssemblyAttribute"/> attribute.
    /// </summary>
    public Assembly[] AllAssemblies { get; }
    public bool IsLoaded { get; set; }

    public BonModuleDescriptor(Type moduleType , IBonModule instance, bool isLoaded)
    {
        ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
        Dependencies = new List<BonModuleDescriptor>();
        AllAssemblies = BonyanModuleHelper.GetAllAssemblies(moduleType);
        Instance = instance;
        IsLoaded = isLoaded;
    }
    
    
    public void AddDependency(BonModuleDescriptor descriptor)
    {
        Dependencies.AddIfNotContains(descriptor);
    }

}