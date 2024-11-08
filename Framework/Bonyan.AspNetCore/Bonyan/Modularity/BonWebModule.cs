using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

public abstract class BonWebModule : BonModule, IWebModule
{
    public virtual Task OnPreApplicationAsync(BonContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnApplicationAsync(BonContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnPostApplicationAsync(BonContext context)
    {
        return Task.CompletedTask;
    }
}