using Bonyan.Modularity;
using Bonyan.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.AspNetCore.Components;

public class BonAspNetCoreComponentsModule : BonWebModule
{
    public BonAspNetCoreComponentsModule()
    {
        DependOn<BonAspNetCoreModule>();
    }


    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddSingleton<ThemeService>();
        context.Services.AddSingleton<MenuService>();
        return base.OnPreConfigureAsync(context);
    }
}