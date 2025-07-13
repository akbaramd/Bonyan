using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Menus;
using Assets;
using Microsoft.Extensions.DependencyInjection;

public class BonUiNovinoModule : BonModule
{
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
        return base.OnInitializeAsync(context);
    }

    private void ConfigureMenuServices(BonConfigurationContext context)
    {
        // Register menu services
        context.Services.AddScoped<IMenuManager, MenuManager>();
        
        // Configure menu services with fluent API
        context.ConfigureMenus(config =>
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
        var menuManager = context.GetRequireService<IMenuManager>();
        
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