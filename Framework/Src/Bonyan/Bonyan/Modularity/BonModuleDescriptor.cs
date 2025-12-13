using System.Reflection;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

/// <summary>
/// Represents metadata about a module, including its type, dependencies, and assemblies.
/// Part of the microkernel core architecture.
/// </summary>
public class BonModuleDescriptor
{
    private readonly List<BonModuleDescriptor> _dependencies;

    /// <summary>
    /// Gets the type of the module.
    /// </summary>
    public Type ModuleType { get; }

    /// <summary>
    /// Gets or sets the module instance.
    /// </summary>
    public IBonModule? Instance { get; set; }

    /// <summary>
    /// Gets the list of module dependencies (read-only).
    /// </summary>
    public IReadOnlyList<BonModuleDescriptor> Dependencies => _dependencies.AsReadOnly();

    /// <summary>
    /// Gets or sets the service name for this module.
    /// </summary>
    public string ServiceName { get; set; }

    /// <summary>
    /// Gets the main assembly of the module.
    /// </summary>
    public Assembly Assembly { get; }

    /// <summary>
    /// Gets all assemblies associated with the module.
    /// Includes the main <see cref="Assembly"/> and other assemblies defined
    /// on the module <see cref="Type"/> using the <see cref="AdditionalAssemblyAttribute"/> attribute.
    /// </summary>
    public Assembly[] AllAssemblies { get; }

    /// <summary>
    /// Gets or sets whether the module has been loaded.
    /// </summary>
    public bool IsLoaded { get; set; }

    /// <summary>
    /// Gets whether this module was loaded as a plugin.
    /// </summary>
    public bool IsPluginModule { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="BonModuleDescriptor"/>.
    /// </summary>
    /// <param name="moduleType">The type of the module.</param>
    /// <param name="instance">The module instance (can be null if not yet created).</param>
    /// <param name="isLoaded">Whether the module has been loaded.</param>
    /// <param name="serviceName">The service name for the module.</param>
    /// <param name="isPluginModule">Whether this module was loaded as a plugin.</param>
    public BonModuleDescriptor(
        Type moduleType,
        IBonModule? instance,
        bool isLoaded,
        string serviceName,
        bool isPluginModule = false)
    {
        ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
        _dependencies = new List<BonModuleDescriptor>();
        Assembly = moduleType.Assembly;
        AllAssemblies = BonyanModuleHelper.GetAllAssemblies(moduleType);
        Instance = instance;
        IsLoaded = isLoaded;
        ServiceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
        IsPluginModule = isPluginModule;
    }

    /// <summary>
    /// Adds a dependency to this module descriptor.
    /// </summary>
    /// <param name="descriptor">The module descriptor to depend on.</param>
    /// <returns>This instance for fluent API support.</returns>
    /// <exception cref="ArgumentNullException">Thrown if descriptor is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if module depends on itself or circular dependency detected.</exception>
    public BonModuleDescriptor AddDependency(BonModuleDescriptor descriptor)
    {
        if (descriptor == null)
            throw new ArgumentNullException(nameof(descriptor));

        if (descriptor == this)
            throw new InvalidOperationException(
                $"Module {ModuleType.FullName} cannot depend on itself.");

        if (_dependencies.Contains(descriptor))
            return this; // Already added, no-op for idempotency

        _dependencies.Add(descriptor);
        return this; // Fluent API support
    }

    /// <summary>
    /// Checks if this module has a dependency on the specified descriptor.
    /// </summary>
    /// <param name="descriptor">The descriptor to check.</param>
    /// <returns>True if this module depends on the specified descriptor; otherwise, false.</returns>
    public bool HasDependency(BonModuleDescriptor descriptor)
    {
        return _dependencies.Contains(descriptor);
    }
}