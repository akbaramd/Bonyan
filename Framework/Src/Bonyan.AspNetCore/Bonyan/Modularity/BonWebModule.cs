using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

public abstract class BonWebModule : BonModule, IWebModule
{
    public virtual Task OnPreApplicationAsync(BonWebApplicationContext webApplicationContext)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnApplicationAsync(BonWebApplicationContext webApplicationContext)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnPostApplicationAsync(BonWebApplicationContext webApplicationContext)
    {
        return Task.CompletedTask;
    }
}