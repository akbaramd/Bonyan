using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

public abstract class BonWebModule : BonModule, IWebModule
{
    public virtual Task OnPreApplicationAsync(BonWebApplicationContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnApplicationAsync(BonWebApplicationContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        return Task.CompletedTask;
    }
}