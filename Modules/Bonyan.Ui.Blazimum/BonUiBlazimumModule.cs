using Bonyan.AspNetCore.Components;
using Bonyan.Modularity;

namespace Bonyan.AdminLte;

public class BonUiBlazimumModule : BonWebModule 
{
    public BonUiBlazimumModule()
    {
        DependOn<BonAspNetCoreComponentsModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        Configure<BonBlazorOptions>(c =>
        {
            c.AddAssembly<BonUiBlazimumModule>();
        });

        return base.OnConfigureAsync(context);
    }
}