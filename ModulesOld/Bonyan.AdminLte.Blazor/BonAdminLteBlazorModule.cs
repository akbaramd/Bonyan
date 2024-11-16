using Bonyan.AspNetCore.Components;
using Bonyan.Modularity;

namespace Bonyan.AdminLte;

public class BonAdminLteBlazorModule : BonWebModule 
{
    public BonAdminLteBlazorModule()
    {
        DependOn<BonAspNetCoreComponentsModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        Configure<BonBlazorOptions>(c =>
        {
            c.AddAssembly<BonAdminLteBlazorModule>();
        });

        return base.OnConfigureAsync(context);
    }
}