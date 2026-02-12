using Bonyan.AspNetCore.Mvc;
using Bonyan.Modularity;
using Bonyan.Ui.BonWeb.Mvc.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Ui.BonWeb.Mvc;

/// <summary>
/// BonWeb MVC module - provides base dashboard layout, menu providers, and asset providers.
/// </summary>
public class BonWebMvcModule : BonWebModule
{
    public BonWebMvcModule()
    {
        DependOn<BonAspNetCoreMvcModule>();
    }

    public override ValueTask OnPreConfigureAsync(BonPreConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.Services.PreConfigure<IMvcBuilder>(builder =>
        {
            builder.AddApplicationPart(typeof(BonWebMvcModule).Assembly);
        });
        return base.OnPreConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.Services.AddSingleton<IBonWebMenuManager, BonWebMenuManager>();
        context.Services.AddSingleton<IBonWebAssetManager, BonWebAssetManager>();
        context.Services.AddSingleton<IBonWebMenuProvider, BonWebDefaultMenuProvider>();
        context.Services.AddSingleton<IBonWebAssetProvider, BonWebDefaultAssetProvider>();
        return base.OnConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnInitializeAsync(BonInitializedContext context, CancellationToken cancellationToken = default)
    {
        var menuManager = context.GetRequiredService<IBonWebMenuManager>();
        var assetManager = context.GetRequiredService<IBonWebAssetManager>();

        foreach (var provider in context.GetServices<IBonWebMenuProvider>())
            menuManager.RegisterProvider(provider);

        foreach (var provider in context.GetServices<IBonWebAssetProvider>())
            assetManager.RegisterProvider(provider);

        return base.OnInitializeAsync(context, cancellationToken);
    }

    public override ValueTask OnApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        return base.OnApplicationAsync(context, cancellationToken);
    }
}
