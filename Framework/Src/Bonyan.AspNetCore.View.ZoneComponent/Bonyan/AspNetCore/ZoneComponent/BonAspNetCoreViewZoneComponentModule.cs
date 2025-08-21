using Bonyan.Modularity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.AspNetCore.ZoneComponent;

public class BonAspNetCoreViewZoneComponentModule : BonWebModule
{
    public BonAspNetCoreViewZoneComponentModule()
    {
        DependOn<BonAspNetCoreModule>();
    }
    
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        // Register the zone component logger
        context.Services.TryAddSingleton<IZoneComponentLogger, ZoneComponentLogger>();
        
        // Register the nested view system with proper scoping
        context.Services.TryAddSingleton<IViewEngine>(sp => sp.GetRequiredService<IRazorViewEngine>());
        
        // Register MVC services required for view rendering in zones
        // Only register if not already registered to avoid conflicts
        if (!context.Services.Any(d => d.ServiceType == typeof(ICompositeViewEngine)))
        {
            context.Services.AddMvcCore()
                .AddViews()
                .AddRazorViewEngine();
        }
        
        // Note: IViewBufferScope is automatically registered by AddViews() and AddRazorViewEngine()
        // We don't need to manually register it as it's handled by ASP.NET Core internally
        
        // Register the zone system
        context.Services.AddZoneComponentsFrom(GetType().Assembly);
        
        return base.OnConfigureAsync(context);
    }
}