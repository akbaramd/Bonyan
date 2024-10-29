using System.Reflection;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;

namespace Bonyan.Modularity;

public class ModuleManager : IModuleManager
{
    private readonly IModuleAccessor _moduleAccessor;

    public ModuleManager(IModuleAccessor moduleAccessor)
    {
        _moduleAccessor = moduleAccessor;
    }

    public void LoadModules(Type mainModuleType)
    {
        ValidateMainModuleType(mainModuleType);
        LoadModuleRecursive(mainModuleType);

        var sortedModules = TopologicalSort(_moduleAccessor.GetAllModules().ToList());
        UpdateModuleListAndInstantiate(sortedModules); // Updated method to include instantiation
    }

    private void ValidateMainModuleType(Type mainModuleType)
    {
        if (mainModuleType == null)
            throw new ArgumentNullException(nameof(mainModuleType));

        if (!typeof(IModule).IsAssignableFrom(mainModuleType))
            throw new ArgumentException($"Type {mainModuleType.FullName} does not implement IModule.");
    }

    private void LoadModuleRecursive(Type moduleType)
    {
        if (_moduleAccessor.GetModule(moduleType) != null) return;

        var moduleInfo = new ModuleInfo(moduleType);
        _moduleAccessor.AddModule(moduleInfo);

        foreach (var dependencyType in GetDependencies(moduleType))
        {
            LoadModuleRecursive(dependencyType);
            moduleInfo.Dependencies.Add(_moduleAccessor.GetModule(dependencyType)!);
        }
    }

    private IEnumerable<Type> GetDependencies(Type moduleType)
    {
        return moduleType.GetCustomAttributes<DependOnAttribute>()
                         .SelectMany(attr => attr.DependedTypes)
                         .Where(dependencyType => typeof(IModule).IsAssignableFrom(dependencyType));
    }

    private List<ModuleInfo> TopologicalSort(List<ModuleInfo> modules)
    {
        var sorted = new List<ModuleInfo>();
        var visited = new Dictionary<ModuleInfo, bool>();

        foreach (var module in modules)
        {
            if (!Visit(module, visited, sorted))
                throw new InvalidOperationException("Circular dependency detected.");
        }

        return sorted;
    }

    private bool Visit(ModuleInfo module, Dictionary<ModuleInfo, bool> visited, List<ModuleInfo> sorted)
    {
        if (visited.TryGetValue(module, out var inProcess))
        {
            if (inProcess) return false;
            return true;
        }

        visited[module] = true;

        foreach (var dependency in module.Dependencies)
        {
            if (!Visit(dependency, visited, sorted)) return false;
        }

        visited[module] = false;
        sorted.Add(module);
        return true;
    }

    private void UpdateModuleListAndInstantiate(IEnumerable<ModuleInfo> sortedModules)
    {
        _moduleAccessor.ClearModules(); // Clear existing modules to refresh with sorted order
        foreach (var module in sortedModules)
        {
            module.Instance = (IModule?)Activator.CreateInstance(module.ModuleType); // Create instance and assign
            _moduleAccessor.AddModule(module); // Add each sorted module to the accessor
        }
    }

 
}
