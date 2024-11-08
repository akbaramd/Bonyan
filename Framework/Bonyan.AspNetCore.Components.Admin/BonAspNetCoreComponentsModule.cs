using Bonyan.AspNetCore.Components.Admin.Middlewares;
using Bonyan.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.AspNetCore.Components.Admin;

public class BonAspNetCoreComponentsAdminModule : BonWebModule
{
    public BonAspNetCoreComponentsAdminModule()
    {
        DependOn<BonAspNetCoreComponentsModule>();
    }

    public override Task OnApplicationAsync(BonContext context)
    {
        var themeService = context.Application.Services.GetRequiredService<ThemeService>();
        themeService.MenuService.RegisterMenuLocation("sidebar");
        themeService.MenuService.RegisterMenuLocation("footer");
        context.Application.UseMiddleware<AssetInjectionMiddleware>();
        return base.OnApplicationAsync(context);
    }
}