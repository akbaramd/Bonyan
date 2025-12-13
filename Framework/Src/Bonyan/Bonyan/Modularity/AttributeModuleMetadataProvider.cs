using System.Collections.Concurrent;
using System.Reflection;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.Logging;

namespace Bonyan.Modularity;

/// <summary>
/// Provides module metadata by reading attributes and static information.
/// Avoids instantiating modules just to read dependencies, supporting microkernel isolation.
/// </summary>
public sealed class AttributeModuleMetadataProvider : IModuleMetadataProvider
{
    private readonly ConcurrentDictionary<Type, ModuleMetadata> _cache = new();
    private readonly ILogger<AttributeModuleMetadataProvider>? _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="AttributeModuleMetadataProvider"/>.
    /// </summary>
    /// <param name="logger">Optional logger for diagnostic information.</param>
    public AttributeModuleMetadataProvider(ILogger<AttributeModuleMetadataProvider>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the dependencies for a module type from static metadata (attributes).
    /// Note: Constructor-based dependencies (DependOn() calls) are read separately
    /// and merged in BonModuleLoader after module instantiation.
    /// </summary>
    /// <param name="moduleType">The module type to get dependencies for.</param>
    /// <returns>Collection of module types this module depends on (from attributes only).</returns>
    public IReadOnlyList<Type> GetDependencies(Type moduleType)
    {
        return GetMetadata(moduleType).Dependencies;
    }

    /// <summary>
    /// Gets all assemblies associated with a module type.
    /// </summary>
    /// <param name="moduleType">The module type to get assemblies for.</param>
    /// <returns>Collection of assemblies.</returns>
    public IReadOnlyList<System.Reflection.Assembly> GetAssemblies(Type moduleType)
    {
        return GetMetadata(moduleType).Assemblies;
    }

    /// <summary>
    /// Gets complete metadata for a module type from static sources (attributes), using caching for performance.
    /// This reads only from attributes - constructor-based dependencies (DependOn() calls) are handled separately
    /// in BonModuleLoader after module instantiation and merged with attribute-based dependencies.
    /// </summary>
    /// <param name="moduleType">The module type.</param>
    /// <returns>Module metadata (attribute-based dependencies only).</returns>
    public ModuleMetadata GetMetadata(Type moduleType)
    {
        return _cache.GetOrAdd(moduleType, type =>
        {
            // Read dependencies from attributes (static metadata - no instantiation required)
            var dependencies = new List<Type>();
            var dependencyAttributes = type.GetCustomAttributes<DependsOnAttribute>(inherit: true);
            foreach (var attr in dependencyAttributes)
            {
                if (attr.ModuleType != null)
                {
                    dependencies.Add(attr.ModuleType);
                }
            }

            // Also check for other attributes that implement IDependedTypesProvider (for extensibility)
            var allAttributes = type.GetCustomAttributes(inherit: true);
            var providerAttributes = allAttributes
                .OfType<IDependedTypesProvider>()
                .Where(a => a is not DependsOnAttribute); // Avoid duplicates
            foreach (var provider in providerAttributes)
            {
                var providedTypes = provider.GetDependedTypes();
                foreach (var depType in providedTypes)
                {
                    if (depType != null && !dependencies.Contains(depType))
                    {
                        dependencies.Add(depType);
                    }
                }
            }

            // Get assemblies
            var assemblies = new List<System.Reflection.Assembly> { type.Assembly };
            var additionalAssemblyAttributes = type.GetCustomAttributes<AdditionalAssemblyAttribute>(inherit: true);
            foreach (var attr in additionalAssemblyAttributes)
            {
                var attrAssemblies = attr.GetAssemblies();
                foreach (var assembly in attrAssemblies)
                {
                    if (!assemblies.Contains(assembly))
                    {
                        assemblies.Add(assembly);
                    }
                }
            }

            return new ModuleMetadata(type, dependencies, assemblies);
        });
    }
}

