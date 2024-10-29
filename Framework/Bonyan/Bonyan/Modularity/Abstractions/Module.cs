using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity.Abstractions;

public abstract class Module : IModule
{


    public virtual Task OnPreConfigureAsync(ServiceConfigurationContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnConfigureAsync(ServiceConfigurationContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnPostConfigureAsync(ServiceConfigurationContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnPreInitializeAsync(ServiceInitializationContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnInitializeAsync(ServiceInitializationContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnPostInitializeAsync(ServiceInitializationContext context)
    {
        return Task.CompletedTask;
    }


    internal static void CheckBonyanModuleType(Type moduleType)
    {
      if (!IsBonyanModule(moduleType))
      {
        throw new ArgumentException("Given type is not an ABP module: " + moduleType.AssemblyQualifiedName);
      }
    }
    
    public static bool IsBonyanModule(Type type)
    {
      var typeInfo = type.GetTypeInfo();

      return
        typeInfo.IsClass &&
        !typeInfo.IsAbstract &&
        !typeInfo.IsGenericType &&
        typeof(IModule).GetTypeInfo().IsAssignableFrom(type);
    }
}