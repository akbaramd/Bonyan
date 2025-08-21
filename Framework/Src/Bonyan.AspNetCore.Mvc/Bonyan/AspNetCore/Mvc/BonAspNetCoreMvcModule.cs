using Bonyan.AspNetCore.Mvc.Localization;
using Bonyan.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Bonyan.AspNetCore.Mvc;

public class BonAspNetCoreMvcModule : BonWebModule
{
    public BonAspNetCoreMvcModule()
    {
        DependOn<BonAspNetCoreModule>();
    }
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        
        var abpMvcDataAnnotationsLocalizationOptions = context.Services
            .ExecutePreConfiguredActions(
                new BonMvcDataAnnotationsLocalizationOptions()
            );
        
        var builder = context.Services.AddMvc()
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var resourceType = abpMvcDataAnnotationsLocalizationOptions
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

        Configure<BonEndpointRouterOptions>(options =>
        {
            options.EndpointConfigureActions.Add(endpointContext =>
            {
                // 1️⃣ Default route first
                endpointContext.Endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );

                // 2️⃣ Area route only after default
                endpointContext.Endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                endpointContext.Endpoints.MapRazorPages();
            });
        });
        return base.OnConfigureAsync(context);
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
    /// Common view location patterns that apply to all modules
    /// </summary>
    public List<string> CommonViewLocations { get; set; } = new()
    {
        // Generic module patterns
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
        
        // Cross-module view patterns (for components from other modules)
        "/Areas/RoleManagement/Pages/Shared/{0}.cshtml",
        "/Areas/RoleManagement/Views/Shared/{0}.cshtml",
        "/Areas/RoleManagement/ZoneViews/Shared/{0}.cshtml",
        "/Areas/UserManagement/Pages/Shared/{0}.cshtml",
        "/Areas/UserManagement/Views/Shared/{0}.cshtml",
        "/Areas/UserManagement/ZoneViews/Shared/{0}.cshtml",
        
        // Handle "Shared/" prefix in view names - when view name is "Shared/_UserRolesPartial"
        "/Areas/RoleManagement/Pages/Shared/Shared/{0}.cshtml",
        "/Areas/RoleManagement/Views/Shared/Shared/{0}.cshtml",
        "/Areas/RoleManagement/ZoneViews/Shared/Shared/{0}.cshtml",
        "/Areas/UserManagement/Pages/Shared/Shared/{0}.cshtml",
        "/Areas/UserManagement/Views/Shared/Shared/{0}.cshtml",
        "/Areas/UserManagement/ZoneViews/Shared/Shared/{0}.cshtml",
        
        // Handle direct view names without "Shared/" prefix
        "/Areas/RoleManagement/Pages/Shared/{0}.cshtml",
        "/Areas/RoleManagement/Views/Shared/{0}.cshtml",
        "/Areas/RoleManagement/ZoneViews/Shared/{0}.cshtml",
        "/Areas/UserManagement/Pages/Shared/{0}.cshtml",
        "/Areas/UserManagement/Views/Shared/{0}.cshtml",
        "/Areas/UserManagement/ZoneViews/Shared/{0}.cshtml",
        
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
        // Combine module-specific locations, common locations, and existing locations
        var allLocations = new List<string>();
        
        // Add module-specific view locations
        allLocations.AddRange(_options.ModuleViewLocations);
        
        // Add common view locations
        allLocations.AddRange(_options.CommonViewLocations);
        
        // Add existing view locations
        allLocations.AddRange(viewLocations);

        // Debug logging - always log to console for now
        Console.WriteLine($"ModuleViewLocationExpander: Called for view '{context.ViewName}'");
        Console.WriteLine($"ModuleViewLocationExpander: Area: {context.AreaName}");
        Console.WriteLine($"ModuleViewLocationExpander: Controller: {context.ControllerName}");
        Console.WriteLine($"ModuleViewLocationExpander: Module locations ({_options.ModuleViewLocations.Count}): {string.Join(", ", _options.ModuleViewLocations)}");
        Console.WriteLine($"ModuleViewLocationExpander: Common locations ({_options.CommonViewLocations.Count}): {string.Join(", ", _options.CommonViewLocations)}");
        Console.WriteLine($"ModuleViewLocationExpander: Total locations: {allLocations.Count}");
        Console.WriteLine($"ModuleViewLocationExpander: First 10 locations: {string.Join(", ", allLocations.Take(10))}");

        // Also try to get logger if available
        var logger = context.ActionContext?.HttpContext?.RequestServices?.GetService<ILogger<ModuleViewLocationExpander>>();
        if (logger != null)
        {
            logger.LogDebug("ModuleViewLocationExpander: Module locations: {ModuleLocations}", 
                string.Join(", ", _options.ModuleViewLocations));
            logger.LogDebug("ModuleViewLocationExpander: Common locations: {CommonLocations}", 
                string.Join(", ", _options.CommonViewLocations));
            logger.LogDebug("ModuleViewLocationExpander: Total locations: {TotalLocations}", 
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
        
        // Register the expander as a singleton service
        services.AddSingleton<ModuleViewLocationExpander>();
        
        // Register a startup filter to add the expander to the view engine
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
/// Startup filter to register the module view location expander with the Razor view engine
/// </summary>
public class ModuleViewLocationExpanderStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            // Get the expander from the service provider
            var expander = app.ApplicationServices.GetRequiredService<ModuleViewLocationExpander>();
            
            // Get the Razor view engine options and add our expander
            var viewEngineOptions = app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Options.IOptions<RazorViewEngineOptions>>();
            viewEngineOptions.Value.ViewLocationExpanders.Add(expander);
            
            next(app);
        };
    }
}