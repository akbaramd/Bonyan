using System.Reflection;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

internal static class BonyanModuleHelper
{
    public static List<Type> FindAllModuleTypes(Type startupModuleType)
    {
        var moduleTypes = new List<Type>();
        Console.WriteLine( "Loaded Bonyan modules:");
        AddModuleAndDependenciesRecursively(moduleTypes, startupModuleType);
        return moduleTypes;
    }

    public static List<Type> FindDependedModuleTypes(Type moduleType)
    {
        BonModule.CheckBonyanModuleType(moduleType);

        var dependencies = new List<Type>();

        // Retrieve dependencies from custom attributes
        var dependencyDescriptors = moduleType
            .GetCustomAttributes()
            .OfType<IDependedTypesProvider>();

        foreach (var descriptor in dependencyDescriptors)
        {
            foreach (var dependedModuleType in descriptor.GetDependedTypes())
            {
                dependencies.AddIfNotContains(dependedModuleType);
            }
        }

        // Initialize the module instance and extract DependedModules
        IBonModule? moduleInstance = null;
        try
        {
            moduleInstance = Activator.CreateInstance(moduleType) as IBonModule;
            if (moduleInstance != null)
            {
                var instanceDependencies = moduleInstance.DependedModules;
                foreach (var dependedModuleType in instanceDependencies)
                {
                    dependencies.AddIfNotContains(dependedModuleType);
                }
            }
        }
        finally
        {
            if (moduleInstance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

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
        Console.WriteLine($"{new string(' ', depth * 2)}- {moduleType.FullName}");

        foreach (var dependedModuleType in FindDependedModuleTypes(moduleType))
        {
            AddModuleAndDependenciesRecursively(moduleTypes, dependedModuleType, depth + 1);
        }
    }
}
