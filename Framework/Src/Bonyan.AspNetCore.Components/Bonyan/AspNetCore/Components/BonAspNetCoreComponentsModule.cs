using Bonyan.Modularity;

namespace Bonyan.AspNetCore.Components;

public class BonAspNetCoreComponentsModule : BonWebModule
{
    public BonAspNetCoreComponentsModule()
    {
        DependOn<BonAspNetCoreModule>();
    }


    public override ValueTask OnPreConfigureAsync(BonPreConfigurationContext context , CancellationToken cancellationToken = default)
    {
        return base.OnPreConfigureAsync(context);
    }
}