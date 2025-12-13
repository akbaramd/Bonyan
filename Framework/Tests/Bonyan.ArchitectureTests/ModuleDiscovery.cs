using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.ArchitectureTests;

/// <summary>
/// Discovers and analyzes modules from assemblies.
/// </summary>
internal static class ModuleDiscovery
{
    /// <summary>
    /// Finds all module types from the given assemblies.
    /// </summary>
    /// <param name="assemblies">Assemblies to search.</param>
    /// <param name="moduleInterface">The module interface/base type to find.</param>
    /// <returns>List of module types.</returns>
    public static IReadOnlyList<Type> FindModuleTypes(IEnumerable<Assembly> assemblies, Type moduleInterface)
    {
        return assemblies
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Some types may fail to load - return the successfully loaded ones
                    return ex.Types.Where(t => t != null)!;
                }
                catch
                {
                    return Enumerable.Empty<Type>();
                }
            })
            .Where(t => t is { IsClass: true, IsAbstract: false } && moduleInterface.IsAssignableFrom(t))
            .ToList();
    }

    /// <summary>
    /// Gets dependencies for a module type from [DependsOn] attributes.
    /// </summary>
    /// <param name="moduleType">The module type to analyze.</param>
    /// <returns>List of depended module types.</returns>
    public static IReadOnlyList<Type> GetDependencies(Type moduleType)
    {
        return moduleType
            .GetCustomAttributes(inherit: true)
            .OfType<DependsOnAttribute>()
            .Select(a => a.ModuleType)
            .Distinct()
            .ToList();
    }

    /// <summary>
    /// Gets all [DependsOn] attributes for a module type, including Reason.
    /// </summary>
    /// <param name="moduleType">The module type to analyze.</param>
    /// <returns>List of dependency attributes.</returns>
    public static IReadOnlyList<DependsOnAttribute> GetDependencyAttributes(Type moduleType)
    {
        return moduleType
            .GetCustomAttributes(inherit: true)
            .OfType<DependsOnAttribute>()
            .ToList();
    }

    /// <summary>
    /// Determines if an assembly is a plugin assembly.
    /// </summary>
    /// <param name="asm">Assembly to check.</param>
    /// <returns>True if the assembly is a plugin.</returns>
    public static bool IsPluginAssembly(Assembly asm)
    {
        var name = asm.GetName().Name ?? "";
        return name.StartsWith(ArchitectureTestConstants.PluginPrefix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines if an assembly is a contracts-only assembly.
    /// </summary>
    /// <param name="asm">Assembly to check.</param>
    /// <returns>True if the assembly is contracts-only.</returns>
    public static bool IsContractsAssembly(Assembly asm)
    {
        var name = asm.GetName().Name ?? "";
        return name.EndsWith(ArchitectureTestConstants.ContractsSuffix, StringComparison.OrdinalIgnoreCase) ||
               name.EndsWith(ArchitectureTestConstants.AbstractionsSuffix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines if an assembly is a core/shared assembly.
    /// </summary>
    /// <param name="asm">Assembly to check.</param>
    /// <returns>True if the assembly is core/shared.</returns>
    public static bool IsCoreAssembly(Assembly asm)
    {
        var name = asm.GetName().Name ?? "";
        return ArchitectureTestConstants.AllowedSharedAssemblies.Contains(name);
    }
}

