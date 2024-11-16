using Bonyan.Modularity;

namespace Bonyan.AspNetCore.Components;

public class BonAspNetCoreComponentsModule : BonWebModule
{
    public BonAspNetCoreComponentsModule()
    {
        DependOn<BonAspNetCoreModule>();
    }


    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        return base.OnPreConfigureAsync(context);
    }
}