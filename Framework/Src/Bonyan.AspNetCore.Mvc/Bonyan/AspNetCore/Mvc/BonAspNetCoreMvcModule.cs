using System;
using System.Collections.Generic;
using Bonyan.AspNetCore.Mvc.Localization;
using Bonyan.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Bonyan.AspNetCore.Mvc;

/// <summary>
/// ASP.NET Core MVC module for Bonyan framework.
/// Configures MVC services, view localization, and routing.
/// </summary>
public class BonAspNetCoreMvcModule : BonWebModule
{
    public BonAspNetCoreMvcModule()
    {
        DependOn<BonAspNetCoreModule>();
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();
        
        // Execute all pre-configured actions from OnPreConfigureAsync phase
        var mvcDataAnnotationsLocalizationOptions = context.ExecutePreConfiguredActions(
            new BonMvcDataAnnotationsLocalizationOptions()
        );
        
        var builder = context.Services.AddMvc()
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var resourceType = mvcDataAnnotationsLocalizationOptions
                        .AssemblyResources
                        .GetOrDefault(type.Assembly);

                    if (resourceType != null)
                    {
                        return factory.Create(resourceType);
                    }

                    return factory.CreateDefaultOrNull() ??
                           factory.Create(type);
                };
            })
            .AddViewLocalization();
        
        builder.AddControllersAsServices();
        builder.AddViewComponentsAsServices();

        // Configure module view location expander
        context.Services.AddModuleViewLocationExpander();

        context.Services.ExecutePreConfiguredActions(builder);

        context.ConfigureOptions<BonEndpointRouterOptions>(options =>
        {
            options.ConfigureActions.Add(endpoints =>
            {
                // 1️⃣ Default route first
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );

                // 2️⃣ Area route only after default
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapRazorPages();
            });
        });
        
        return base.OnConfigureAsync(context, cancellationToken);
    }
}

/// <summary>
/// Options for module view location expander
/// </summary>
public class ModuleViewLocationExpanderOptions
{
    /// <summary>
    /// Collection of view locations registered by modules
    /// </summary>
    public List<string> ModuleViewLocations { get; set; } = new();

    /// <summary>
    /// Global (non-area) view locations that are always searched so shared partials
    /// from RCLs (e.g. Views/Shared) are found even when the request is in an area.
    /// </summary>
    public List<string> GlobalViewLocations { get; set; } = new()
    {
        "Views/{1}/{0}.cshtml",
        "Views/Shared/{0}.cshtml",
        "Pages/Shared/{0}.cshtml"
    };

    /// <summary>
    /// Common view location patterns that apply to all modules.
    /// These are generic patterns that work for any module using the {2} placeholder for area name.
    /// Module-specific patterns should be added via AddModuleViewLocations or AddStandardModuleViewLocations.
    /// </summary>
    public List<string> CommonViewLocations { get; set; } = new()
    {
        // Generic module patterns (using {2} for area name - works for all modules)
        "/Areas/{2}/Pages/{1}/{0}.cshtml",
        "/Areas/{2}/Pages/{1}/Partials/{0}.cshtml",
        "/Areas/{2}/Pages/{1}/Shared/{0}.cshtml",
        "/Areas/{2}/Pages/Shared/{0}.cshtml",
        "/Areas/{2}/Views/{1}/{0}.cshtml",
        "/Areas/{2}/Views/Shared/{0}.cshtml",
        "/Areas/{2}/Views/Shared/Components/{1}/{0}.cshtml",
        "/Areas/{2}/Pages/Shared/Components/{1}/{0}.cshtml",
        
        // Zone-specific patterns
        "/Areas/{2}/ZoneViews/{0}.cshtml",
        "/Areas/{2}/ZoneViews/Tabs/{0}.cshtml",
        "/Areas/{2}/ZoneViews/Shared/{0}.cshtml",
        
        // Embedded resource patterns (for compiled views)
        "/Areas/{2}/Pages/{1}/{0}.cshtml",
        "/Areas/{2}/Views/{1}/{0}.cshtml"
    };

    /// <summary>
    /// Add view locations for a specific module
    /// </summary>
    /// <param name="moduleName">The module name</param>
    /// <param name="viewLocations">The view locations to add</param>
    public void AddModuleViewLocations(string moduleName, params string[] viewLocations)
    {
        if (string.IsNullOrEmpty(moduleName)) return;

        foreach (var location in viewLocations)
        {
            var moduleLocation = location.Replace("{moduleName}", moduleName);
            if (!ModuleViewLocations.Contains(moduleLocation))
            {
                ModuleViewLocations.Add(moduleLocation);
            }
        }
    }

    /// <summary>
    /// Add view locations for a specific module with standard patterns
    /// </summary>
    /// <param name="moduleName">The module name</param>
    public void AddStandardModuleViewLocations(string moduleName)
    {
        if (string.IsNullOrEmpty(moduleName)) return;

        var standardLocations = new[]
        {
            // Main module pages
            $"/Areas/{moduleName}/Pages/{{1}}/{{0}}.cshtml",
            $"/Areas/{moduleName}/Pages/{{1}}/Partials/{{0}}.cshtml",
            $"/Areas/{moduleName}/Pages/{{1}}/Shared/{{0}}.cshtml",
            
            // Module shared views
            $"/Areas/{moduleName}/Pages/Shared/{{0}}.cshtml",
            $"/Areas/{moduleName}/Views/Shared/{{0}}.cshtml",
            $"/Areas/{moduleName}/Views/{{1}}/{{0}}.cshtml",
            
            // Module components
            $"/Areas/{moduleName}/Views/Shared/Components/{{1}}/{{0}}.cshtml",
            $"/Areas/{moduleName}/Pages/Shared/Components/{{1}}/{{0}}.cshtml",
            
            // Zone views
            $"/Areas/{moduleName}/ZoneViews/{{0}}.cshtml",
            $"/Areas/{moduleName}/ZoneViews/Tabs/{{0}}.cshtml",
            $"/Areas/{moduleName}/ZoneViews/Shared/{{0}}.cshtml"
        };

        AddModuleViewLocations(moduleName, standardLocations);
    }

    /// <summary>
    /// Add custom view location pattern
    /// </summary>
    /// <param name="viewLocation">The view location pattern</param>
    public void AddCustomViewLocation(string viewLocation)
    {
        if (!string.IsNullOrEmpty(viewLocation) && !ModuleViewLocations.Contains(viewLocation))
        {
            ModuleViewLocations.Add(viewLocation);
        }
    }

    /// <summary>
    /// Add custom common view location pattern
    /// </summary>
    /// <param name="viewLocation">The view location pattern</param>
    public void AddCommonViewLocation(string viewLocation)
    {
        if (!string.IsNullOrEmpty(viewLocation) && !CommonViewLocations.Contains(viewLocation))
        {
            CommonViewLocations.Add(viewLocation);
        }
    }
}

/// <summary>
/// View location expander for module system to find views in module areas
/// </summary>
public class ModuleViewLocationExpander : IViewLocationExpander
{
    private readonly ModuleViewLocationExpanderOptions _options;

    public ModuleViewLocationExpander(Microsoft.Extensions.Options.IOptions<ModuleViewLocationExpanderOptions> options)
    {
        _options = options.Value;
    }

    public void PopulateValues(ViewLocationExpanderContext context)
    {
        // No additional values needed
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        // Combine: global (non-area) first so RCL shared partials are found when in an area,
        // then module-specific, common, and existing (area) locations
        var allLocations = new List<string>();
        
        // Always include global view locations so Views/Shared partials from RCLs are found
        // even when the current request is in an area (area-only formats would otherwise omit them)
        allLocations.AddRange(_options.GlobalViewLocations);
        
        // Add module-specific view locations
        allLocations.AddRange(_options.ModuleViewLocations);
        
        // Add common view locations
        allLocations.AddRange(_options.CommonViewLocations);
        
        // Add existing view locations (from view engine; may be area-only when context has area)
        allLocations.AddRange(viewLocations);

        // Use proper logging instead of Console.WriteLine
        var logger = context.ActionContext?.HttpContext?.RequestServices?.GetService<ILogger<ModuleViewLocationExpander>>();
        if (logger != null && logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace(
                "ModuleViewLocationExpander: Expanding view locations for view '{ViewName}' in area '{AreaName}' controller '{ControllerName}'. " +
                "Module locations: {ModuleLocationCount}, Common locations: {CommonLocationCount}, Total: {TotalLocationCount}",
                context.ViewName,
                context.AreaName ?? "(none)",
                context.ControllerName ?? "(none)",
                _options.ModuleViewLocations.Count,
                _options.CommonViewLocations.Count,
                allLocations.Count);
        }

        return allLocations;
    }
}

/// <summary>
/// Extension methods for configuring module view location expander
/// </summary>
public static class ModuleViewLocationExpanderExtensions
{
    /// <summary>
    /// Add module view location expander to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure the options</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddModuleViewLocationExpander(
        this IServiceCollection services,
        Action<ModuleViewLocationExpanderOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddScoped<IViewLocationExpander, ModuleViewLocationExpander>();
        return services;
    }

    /// <summary>
    /// Add module view location expander to the service collection with default configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddModuleViewLocationExpander(this IServiceCollection services)
    {
        services.Configure<ModuleViewLocationExpanderOptions>(options =>
        {
            // Default configuration is already set in the options constructor
        });
        
        // Register the expander as a scoped service (IViewLocationExpander is used per-request)
        // Also register as singleton for IStartupFilter access
        services.AddSingleton<ModuleViewLocationExpander>();
        services.AddScoped<IViewLocationExpander>(sp => sp.GetRequiredService<ModuleViewLocationExpander>());
        
        // Register a startup filter to add the expander to the view engine
        // Note: IStartupFilter works in .NET 6+ but runs during app configuration
        services.AddSingleton<IStartupFilter, ModuleViewLocationExpanderStartupFilter>();
        
        return services;
    }

    /// <summary>
    /// Add view locations for a specific module
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="moduleName">The module name</param>
    /// <param name="viewLocations">The view locations to add</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddModuleViewLocations(
        this IServiceCollection services,
        string moduleName,
        params string[] viewLocations)
    {
        services.Configure<ModuleViewLocationExpanderOptions>(options =>
        {
            options.AddModuleViewLocations(moduleName, viewLocations);
        });
        return services;
    }

    /// <summary>
    /// Add standard view locations for a specific module
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="moduleName">The module name</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddStandardModuleViewLocations(
        this IServiceCollection services,
        string moduleName)
    {
        services.Configure<ModuleViewLocationExpanderOptions>(options =>
        {
            options.AddStandardModuleViewLocations(moduleName);
        });
        return services;
    }

    /// <summary>
    /// Add custom view location pattern
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="viewLocation">The view location pattern</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddCustomViewLocation(
        this IServiceCollection services,
        string viewLocation)
    {
        services.Configure<ModuleViewLocationExpanderOptions>(options =>
        {
            options.AddCustomViewLocation(viewLocation);
        });
        return services;
    }

    /// <summary>
    /// Add custom common view location pattern
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="viewLocation">The view location pattern</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddCommonViewLocation(
        this IServiceCollection services,
        string viewLocation)
    {
        services.Configure<ModuleViewLocationExpanderOptions>(options =>
        {
            options.AddCommonViewLocation(viewLocation);
        });
        return services;
    }
}

/// <summary>
/// Startup filter to register the module view location expander with the Razor view engine.
/// This ensures the expander is added to the view engine before views are resolved.
/// </summary>
public class ModuleViewLocationExpanderStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            try
            {
                // Get the expander from the service provider
                var expander = app.ApplicationServices.GetRequiredService<ModuleViewLocationExpander>();
                
                // Get the Razor view engine options and add our expander
                var viewEngineOptions = app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Options.IOptions<RazorViewEngineOptions>>();
                
                // Only add if not already added (prevent duplicates)
                if (!viewEngineOptions.Value.ViewLocationExpanders.Contains(expander))
                {
                    viewEngineOptions.Value.ViewLocationExpanders.Add(expander);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail startup - view location expander is optional
                var logger = app.ApplicationServices.GetService<ILogger<ModuleViewLocationExpanderStartupFilter>>();
                logger?.LogWarning(ex, 
                    "Failed to register ModuleViewLocationExpander. View location expansion may not work correctly.");
            }
            
            next(app);
        };
    }
}