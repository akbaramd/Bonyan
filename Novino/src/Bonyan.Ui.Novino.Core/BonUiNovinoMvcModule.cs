using Bonyan.AspNetCore.ZoneComponent;
using Bonyan.IdentityManagement;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Novino.Core.Assets;
using Bonyan.Novino.Core.Menus;
using Bonyan.Ui.Novino.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.Novino.Core;

public class BonNovinoCoreModule<TUser,TRole> : BonModule where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
    public BonNovinoCoreModule()
    {
        DependOn<BonIdentityManagementModule<TUser,TRole>>();
        DependOn<BonAspNetCoreViewZoneComponentModule>();
    }
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        ConfigureMenuServices(context);
        ConfigureAssetServices(context);
        return base.OnConfigureAsync(context);
    }

    public override Task OnInitializeAsync(BonInitializedContext context)
    {
        InitializeMenuProviders(context);
        InitializeAssetProviders(context);
        // Nested view handlers will be initialized by NestedViewInitializer after app is built
        return base.OnInitializeAsync(context);
    }

    private void ConfigureMenuServices(BonConfigurationContext context)
    {
        // Register menu services
        context.Services.AddScoped(typeof(IMenuManager<TUser,TRole>), typeof(MenuManager<TUser,TRole>));
        
        // Configure menu services with fluent API
        context.ConfigureMenus<TUser, TRole>(config =>
        {
            config.AddCommonLocations();
        });
    }

    private void ConfigureAssetServices(BonConfigurationContext context)
    {
        // Configure asset services with fluent API (this registers IAssetManager as Singleton)
        context.ConfigureAssets(config =>
        {
            config.AddCommonAssets();
        });
    }

    private void InitializeMenuProviders(BonInitializedContext context)
    {
        var menuManager = context.GetRequireService<IMenuManager<TUser,TRole>>();
        
        // Register all menu locations
        var menuLocations = context.GetServices<MenuLocation>();
        foreach (var location in menuLocations)
        {
            menuManager.RegisterLocation(location);
        }
        
        // Auto-discover and register all menu providers
        var menuProviders = context.GetServices<IMenuProvider>();
        foreach (var provider in menuProviders)
        {
            menuManager.RegisterProvider(provider);
        }
    }

        private void InitializeAssetProviders(BonInitializedContext context)
    {
        var assetManager = context.GetRequireService<IAssetManager>();
        
        // Auto-discover and register all asset providers
        var assetProviders = context.GetServices<IAssetProvider>();
        foreach (var provider in assetProviders)
        {
            assetManager.RegisterProvider(provider);
        }
    }

   
}