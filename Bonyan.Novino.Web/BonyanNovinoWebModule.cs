
using Bonyan.AspNetCore.Mvc;
using Bonyan.Modularity;
using Menus;
using Assets;
using Microsoft.Extensions.DependencyInjection;
using Bonyan.Novino.Web.Menus;
using Bonyan.Novino.Web.Assets;

namespace Bonyan.Novino.Web;

public class BonyanNovinoWebModule : BonWebModule
{
    public BonyanNovinoWebModule()
    {
        DependOn<BonUiNovinoModule>();
        DependOn<BonAspNetCoreMvcModule>();
    }
    
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddEndpointsApiExplorer();
        
        // Configure menu system
        ConfigureMenuSystem(context);
        
        // Configure asset system
        ConfigureAssetSystem(context);

        return base.OnConfigureAsync(context);
    }

    private void ConfigureMenuSystem(BonConfigurationContext context)
    {
        // Register web-specific menu locations
        context.Services.AddMenuLocation("main-navigation", "Main Navigation", "Primary website navigation menu");
        context.Services.AddMenuLocation("footer-menu", "Footer Menu", "Footer navigation links");
        context.Services.AddMenuLocation("user-menu", "User Menu", "User profile and account menu");
        
        // Register web menu provider
        context.Services.AddMenuProvider<WebMainMenuProvider>();
        context.Services.AddAssetProvider<DefaultAssetProvider>();
    }
    
    private void ConfigureAssetSystem(BonConfigurationContext context)
    {
        // Register default asset provider
        context.Services.AddTransient<IAssetProvider, DefaultAssetProvider>();
    }

    public override Task OnApplicationAsync(BonWebApplicationContext context)
    {
        context.Application.UseCorrelationId();
        return base.OnApplicationAsync(context);
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        context.Application.UseHttpsRedirection();
        return base.OnPostApplicationAsync(context);
    }
}
