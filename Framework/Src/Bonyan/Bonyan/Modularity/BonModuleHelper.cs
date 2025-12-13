using System.Reflection;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

internal static class BonyanModuleHelper
{
    public static List<Type> FindAllModuleTypes(Type startupModuleType)
    {
        var moduleTypes = new List<Type>();
       
        AddModuleAndDependenciesRecursively(moduleTypes, startupModuleType);
        return moduleTypes;
    }

    /// <summary>
    /// Finds depended module types using static metadata (attributes) only.
    /// This avoids instantiating modules just to read dependencies, supporting microkernel isolation.
    /// </summary>
    /// <param name="moduleType">The module type to find dependencies for.</param>
    /// <returns>List of module types this module depends on.</returns>
    public static List<Type> FindDependedModuleTypes(Type moduleType)
    {
        BonModule.CheckBonyanModuleType(moduleType);

        var dependencies = new List<Type>();

        // Retrieve dependencies from custom attributes (static metadata - no instantiation)
        var dependencyDescriptors = moduleType
            .GetCustomAttributes(inherit: true)
            .OfType<IDependedTypesProvider>();

        foreach (var descriptor in dependencyDescriptors)
        {
            foreach (var dependedModuleType in descriptor.GetDependedTypes())
            {
                if (dependedModuleType != null)
                {
                    dependencies.AddIfNotContains(dependedModuleType);
                }
            }
        }

        // Note: Removed module instantiation for reading DependedModules property.
        // Modules should declare dependencies via [DependsOn] attributes for static metadata.
        // This supports microkernel architecture: plug-ins remain isolated, no side effects from constructors.

        return dependencies;
    }
    
    public static Assembly[] GetAllAssemblies(Type moduleType)
    {
        var assemblies = new List<Assembly>();

        var additionalAssemblyDescriptors = moduleType
            .GetCustomAttributes()
            .OfType<IAdditionalModuleAssemblyProvider>();

        foreach (var descriptor in additionalAssemblyDescriptors)
        {
            foreach (var assembly in descriptor.GetAssemblies())
            {
                assemblies.AddIfNotContains(assembly);
            }
        }
        
        assemblies.Add(moduleType.Assembly);

        return assemblies.ToArray();
    }

    private static void AddModuleAndDependenciesRecursively(
        List<Type> moduleTypes,
        Type moduleType,
        int depth = 0)
    {
        BonModule.CheckBonyanModuleType(moduleType);

        if (moduleTypes.Contains(moduleType))
        {
            return;
        }

        moduleTypes.Add(moduleType);
        // Note: Logging removed from helper - should be done by caller (BonModuleLoader) with proper ILogger

        foreach (var dependedModuleType in FindDependedModuleTypes(moduleType))
        {
            AddModuleAndDependenciesRecursively(moduleTypes, dependedModuleType, depth + 1);
        }
    }
}
