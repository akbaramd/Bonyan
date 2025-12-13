using System.Collections.Generic;
using System.Linq;
using Bonyan.Modularity.Abstractions;
using Bonyan.Plugins;
using Microsoft.Extensions.Logging;

namespace Bonyan.Modularity;

/// <summary>
/// Discovers module types from root module, assembly finder, and plugin sources.
/// Part of the microkernel core architecture.
/// </summary>
public sealed class ModuleCatalog : IModuleCatalog
{
    private readonly ILogger<ModuleCatalog>? _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ModuleCatalog"/>.
    /// </summary>
    /// <param name="logger">Optional logger for diagnostic information.</param>
    public ModuleCatalog(ILogger<ModuleCatalog>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Discovers all module types starting from the root module.
    /// </summary>
    /// <param name="rootModuleType">The root module type to start discovery from.</param>
    /// <param name="plugInSources">Plugin sources to discover additional modules from.</param>
    /// <returns>Collection of discovered module types.</returns>
    public IReadOnlyList<Type> DiscoverModuleTypes(Type rootModuleType, PlugInSourceList plugInSources)
    {
        if (rootModuleType == null)
            throw new ArgumentNullException(nameof(rootModuleType));
        if (plugInSources == null)
            throw new ArgumentNullException(nameof(plugInSources));

        var discoveredTypes = new HashSet<Type>();

        // Discover core modules (from root module dependency tree)
        var coreModuleTypes = BonyanModuleHelper.FindAllModuleTypes(rootModuleType);
        foreach (var moduleType in coreModuleTypes)
        {
            discoveredTypes.Add(moduleType);
        }

        // Discover plugin modules
        var pluginModules = plugInSources.GetAllModules();
        if (pluginModules.Any())
        {
            foreach (var moduleType in pluginModules)
            {
                if (discoveredTypes.Contains(moduleType))
                {
                    _logger?.LogWarning(
                        "Plugin module {ModuleType} is already discovered as a core module. Skipping duplicate.",
                        moduleType.FullName);
                    continue;
                }

                discoveredTypes.Add(moduleType);
            }
        }

        return discoveredTypes.ToList();
    }
}

