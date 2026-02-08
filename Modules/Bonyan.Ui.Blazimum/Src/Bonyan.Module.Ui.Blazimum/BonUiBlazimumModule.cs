using Bonyan.AspNetCore.Components;
using Bonyan.Modularity;

namespace Bonyan.Ui.Blazimum;

public class BonUiBlazimumModule : BonWebModule 
{
    public BonUiBlazimumModule()
    {
        DependOn<BonAspNetCoreComponentsModule>();
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
    {
        context.Services.Configure<BonBlazorOptions>(c =>
        {
            c.AddAssembly<BonUiBlazimumModule>();
        });

        return base.OnConfigureAsync(context);
    }
}