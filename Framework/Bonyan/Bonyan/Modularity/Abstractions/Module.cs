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


}