using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

public abstract class WebModule : Module, IWebModule
{
    public virtual Task OnPreApplicationAsync(ApplicationContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnApplicationAsync(ApplicationContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnPostApplicationAsync(ApplicationContext context)
    {
        return Task.CompletedTask;
    }
}