using System;
using System.Collections.Generic;
using System.Linq;
using Bonyan.Core;
using Bonyan.Exceptions;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Plugins;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Volo.Abp.Modularity;

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
        //All modules starting from the startup module
        foreach (var moduleType in BonyanModuleHelper.FindAllModuleTypes(startupModuleType))
        {
            modules.Add(CreateModuleDescriptor(services, moduleType));
        }

        //Plugin modules
        foreach (var moduleType in plugInSources.GetAllModules())
        {
            if (modules.Any(m => m.ModuleType == moduleType))
            {
                continue;
            }

            modules.Add(CreateModuleDescriptor(services, moduleType, isLoadedAsPlugIn: true));
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
