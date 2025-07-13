using System.Linq;
using Bonyan.Core;
using Bonyan.Exceptions;
using Bonyan.Modularity.Abstractions;
using Bonyan.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity;

public class BonModuleLoader : IBonModuleLoader
{
    public BonModuleDescriptor[] LoadModules(
        IServiceCollection services,
        Type startupModuleType,
        PlugInSourceList plugInSources)
    {
        Check.NotNull(services, nameof(services));
        Check.NotNull(startupModuleType, nameof(startupModuleType));
        Check.NotNull(plugInSources, nameof(plugInSources));

        var modules = GetDescriptors(services, startupModuleType, plugInSources);

        modules = SortByDependency(modules, startupModuleType);

        return modules.ToArray();
    }

    private List<BonModuleDescriptor> GetDescriptors(
        IServiceCollection services,
        Type startupModuleType,
        PlugInSourceList plugInSources)
    {
        var modules = new List<BonModuleDescriptor>();

        FillModules(modules, services, startupModuleType, plugInSources);
        SetDependencies(modules);

        return modules.Cast<BonModuleDescriptor>().ToList();
    }

    protected virtual void FillModules(
        List<BonModuleDescriptor> modules,
        IServiceCollection services,
        Type startupModuleType,
        PlugInSourceList plugInSources)
    {
        Console.WriteLine("Loading Bonyan modules:");
        
        //All modules starting from the startup module
        Console.WriteLine("  Core modules:");
        foreach (var moduleType in BonyanModuleHelper.FindAllModuleTypes(startupModuleType))
        {
            modules.Add(CreateModuleDescriptor(services, moduleType));
        }

        //Plugin modules
        var pluginModules = plugInSources.GetAllModules();
        if (pluginModules.Any())
        {
            Console.WriteLine("  Plugin modules:");
            ShowPluginInformation(plugInSources);
            
            foreach (var moduleType in pluginModules)
            {
                if (modules.Any(m => m.ModuleType == moduleType))
                {
                    continue;
                }

                modules.Add(CreateModuleDescriptor(services, moduleType, isLoadedAsPlugIn: true));
                Console.WriteLine($"    - {moduleType.FullName} (Plugin)");
            }
        }

        Console.WriteLine($"Total modules loaded: {modules.Count}");
    }

    /// <summary>
    /// Shows information about loaded plugins from JSON manifests.
    /// </summary>
    private void ShowPluginInformation(PlugInSourceList plugInSources)
    {
        var jsonManifests = new List<PluginManifest>();

        // Collect all JSON manifests from different plugin sources
        foreach (var pluginSource in plugInSources)
        {
            if (pluginSource is JsonPluginSource jsonSource)
            {
                jsonManifests.AddRange(jsonSource.Manifests);
            }
            else if (pluginSource is FolderPlugInSource folderSource)
            {
                jsonManifests.AddRange(folderSource.DiscoveredManifests);
            }
        }

        if (jsonManifests.Any())
        {
            Console.WriteLine("    Discovered plugin manifests:");
            foreach (var manifest in jsonManifests)
            {
                Console.WriteLine($"      📦 {manifest.Name} v{manifest.Version}");
                if (manifest.Authors.Any())
                {
                    Console.WriteLine($"         Authors: {string.Join(", ", manifest.Authors)}");
                }
                if (!string.IsNullOrEmpty(manifest.Description))
                {
                    Console.WriteLine($"         Description: {manifest.Description}");
                }
                Console.WriteLine($"         Entry Point: {manifest.EntryPoint}");
                if (manifest.AdditionalFiles.Any())
                {
                    Console.WriteLine($"         Additional Files: {string.Join(", ", manifest.AdditionalFiles)}");
                }
                if (manifest.Tags.Any())
                {
                    Console.WriteLine($"         Tags: {string.Join(", ", manifest.Tags)}");
                }
            }
        }
    }

    protected virtual void SetDependencies(List<BonModuleDescriptor> modules)
    {
        foreach (var module in modules)
        {
            SetDependencies(modules, module);
        }
    }

    protected virtual List<BonModuleDescriptor> SortByDependency(List<BonModuleDescriptor> modules, Type startupModuleType)
    {
        var sortedModules = modules.SortByDependencies(m => m.Dependencies);
        sortedModules.MoveItem(m => m.ModuleType == startupModuleType, modules.Count - 1);
        return sortedModules;
    }

    protected virtual BonModuleDescriptor CreateModuleDescriptor(IServiceCollection services, Type moduleType, bool isLoadedAsPlugIn = false)
    {
        return new BonModuleDescriptor(moduleType, CreateAndRegisterModule(services, moduleType), isLoadedAsPlugIn);
    }

    protected virtual IBonModule CreateAndRegisterModule(IServiceCollection services, Type moduleType)
    {
        var module = (IBonModule)Activator.CreateInstance(moduleType)!;
        services.AddSingleton(moduleType, module);
        return module;
    }

    protected virtual void SetDependencies(List<BonModuleDescriptor> modules, BonModuleDescriptor bonModule)
    {
        foreach (var dependedModuleType in BonyanModuleHelper.FindDependedModuleTypes(bonModule.ModuleType))
        {
            var dependedModule = modules.FirstOrDefault(m => m.ModuleType == dependedModuleType);
            if (dependedModule == null)
            {
                throw new BonException("Could not find a depended bonModule " + dependedModuleType.AssemblyQualifiedName + " for " + bonModule.ModuleType.AssemblyQualifiedName);
            }

            bonModule.AddDependency(dependedModule);
        }
    }
}
