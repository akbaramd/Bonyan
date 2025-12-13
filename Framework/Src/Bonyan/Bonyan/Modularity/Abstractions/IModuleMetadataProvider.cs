namespace Bonyan.Modularity.Abstractions;

/// <summary>
/// Reads module metadata (dependencies, assemblies) without instantiating modules.
/// Part of the microkernel core architecture.
/// </summary>
public interface IModuleMetadataProvider
{
    /// <summary>
    /// Gets complete metadata for a module type.
    /// </summary>
    /// <param name="moduleType">The module type.</param>
    /// <returns>Module metadata.</returns>
    ModuleMetadata GetMetadata(Type moduleType);

    /// <summary>
    /// Gets the dependencies for a module type without instantiating it.
    /// </summary>
    /// <param name="moduleType">The module type to get dependencies for.</param>
    /// <returns>Collection of module types this module depends on.</returns>
    IReadOnlyList<Type> GetDependencies(Type moduleType);

    /// <summary>
    /// Gets all assemblies associated with a module type.
    /// </summary>
    /// <param name="moduleType">The module type to get assemblies for.</param>
    /// <returns>Collection of assemblies.</returns>
    IReadOnlyList<System.Reflection.Assembly> GetAssemblies(Type moduleType);
}

