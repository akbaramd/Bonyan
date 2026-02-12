using JetBrains.Annotations;

namespace Bonyan.Modularity;

/// <summary>
/// Provides access to loaded module descriptors (ABP-style).
/// Implemented by the modularity application and used by Autofac integration (e.g. property injection, conventions).
/// </summary>
public interface IModuleContainer
{
    /// <summary>
    /// Gets the read-only list of loaded module descriptors.
    /// </summary>
    [NotNull]
    IReadOnlyList<BonModuleDescriptor> Modules { get; }
}
