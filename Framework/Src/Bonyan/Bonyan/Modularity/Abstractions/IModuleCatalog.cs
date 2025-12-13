using Bonyan.Plugins;

namespace Bonyan.Modularity.Abstractions;

/// <summary>
/// Discovers module types from root module, assembly finder, and plugin sources.
/// Part of the microkernel core architecture.
/// </summary>
public interface IModuleCatalog
{
    /// <summary>
    /// Discovers all module types starting from the root module.
    /// </summary>
    /// <param name="rootModuleType">The root module type to start discovery from.</param>
    /// <param name="plugInSources">Plugin sources to discover additional modules from.</param>
    /// <returns>Collection of discovered module types.</returns>
    IReadOnlyList<Type> DiscoverModuleTypes(Type rootModuleType, PlugInSourceList plugInSources);
}

