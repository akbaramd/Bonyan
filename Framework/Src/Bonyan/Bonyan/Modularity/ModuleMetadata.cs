namespace Bonyan.Modularity;

/// <summary>
/// Represents static metadata about a module without instantiating it.
/// Part of the microkernel architecture - keeps plug-ins isolated and metadata accessible.
/// </summary>
public sealed class ModuleMetadata
{
    /// <summary>
    /// Gets the module type.
    /// </summary>
    public Type ModuleType { get; }

    /// <summary>
    /// Gets the module types this module depends on.
    /// </summary>
    public IReadOnlyList<Type> Dependencies { get; }

    /// <summary>
    /// Gets all assemblies associated with this module.
    /// </summary>
    public IReadOnlyList<System.Reflection.Assembly> Assemblies { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ModuleMetadata"/>.
    /// </summary>
    /// <param name="moduleType">The module type.</param>
    /// <param name="dependencies">The module dependencies.</param>
    /// <param name="assemblies">The associated assemblies.</param>
    public ModuleMetadata(
        Type moduleType,
        IReadOnlyList<Type> dependencies,
        IReadOnlyList<System.Reflection.Assembly> assemblies)
    {
        ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
        Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        Assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
    }
}

