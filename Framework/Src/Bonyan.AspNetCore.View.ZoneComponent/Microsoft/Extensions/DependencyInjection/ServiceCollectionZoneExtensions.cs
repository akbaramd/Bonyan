using System.Reflection;
using Bonyan.AspNetCore.ZoneComponent;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering zone components
/// </summary>
public static class ServiceCollectionZoneExtensions
{
    /// <summary>
    /// Add zone components from the specified assembly with automatic discovery
    /// </summary>
    public static IServiceCollection AddZoneComponentsFrom(this IServiceCollection services, Assembly assembly)
    {
        // Register ZoneRegistry if not already registered
        if (!services.Any(d => d.ServiceType == typeof(IZoneRegistry)))
        {
            services.AddSingleton<IZoneRegistry, ZoneRegistry>();
        }

        // Auto-discover and register zone components from assembly
        AutoDiscoverZoneComponents(services, assembly);

        // Register startup filter to register components in registry
        services.AddSingleton<IStartupFilter>(new ZoneStartupFilter());
        
        return services;
    }

    /// <summary>
    /// Add zone components from multiple assemblies
    /// </summary>
    public static IServiceCollection AddZoneComponentsFrom(this IServiceCollection services, params Assembly[] assemblies)
    {
        // Register ZoneRegistry if not already registered
        if (!services.Any(d => d.ServiceType == typeof(IZoneRegistry)))
        {
            services.AddSingleton<IZoneRegistry, ZoneRegistry>();
        }

        // Auto-discover and register zone components from all assemblies
        foreach (var assembly in assemblies)
        {
            AutoDiscoverZoneComponents(services, assembly);
        }

        // Register startup filter to register components in registry
        services.AddSingleton<IStartupFilter>(new ZoneStartupFilter());
        
        return services;
    }

    /// <summary>
    /// Auto-discover zone components from assembly
    /// </summary>
    private static void AutoDiscoverZoneComponents(IServiceCollection services, Assembly assembly)
    {
        // Find all types that implement IZoneComponent (including inherited ones)
        var zoneComponentTypes = assembly.DefinedTypes
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IZoneComponent).IsAssignableFrom(t))
            .ToList();

        var registeredIds = new HashSet<string>();

        foreach (var type in zoneComponentTypes)
        {
            try
            {
                // Check if the type can be instantiated (has parameterless constructor or can be resolved by DI)
                if (CanBeRegistered(type))
                {
                    // Check for duplicate IDs

                    if (registeredIds.Contains(type.FullName))
                    {
                        Console.WriteLine($"Skipped zone component {type.Name}: Duplicate ID '{type.FullName}' already registered");
                        continue;
                    }

                    registeredIds.Add(type.FullName);

                    // Register as scoped by default
                    services.AddScoped(type);
                    services.AddScoped(typeof(IZoneComponent), type);
                    
                    Console.WriteLine($"Registered zone component: {type.Name} (ID: {type.FullName ?? "Unknown"})");
                }
                else
                {
                    Console.WriteLine($"Skipped zone component {type.Name}: Cannot be instantiated");
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with other components
                Console.WriteLine($"Error registering zone component {type.Name}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Create a temporary instance to check the component ID
    /// </summary>
    private static IZoneComponent? CreateInstanceForIdCheck(Type type)
    {
        try
        {
            // Try to create instance using parameterless constructor
            var instance = Activator.CreateInstance(type) as IZoneComponent;
            return instance;
        }
        catch
        {
            // If we can't create instance, we'll skip ID checking
            return null;
        }
    }

    /// <summary>
    /// Check if a type can be safely registered in DI
    /// </summary>
    private static bool CanBeRegistered(Type type)
    {
        try
        {
      
            
            // Or check if it's a concrete class that can be instantiated
            var canBeInstantiated = !type.IsAbstract && !type.IsInterface && type.IsClass;
            
            return  canBeInstantiated;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Register a zone component manually
    /// </summary>
    public static IServiceCollection AddZoneComponent<T>(this IServiceCollection services) where T : class, IZoneComponent
    {
        services.AddScoped<T>();
        services.AddScoped<IZoneComponent, T>();
        return services;
    }

    /// <summary>
    /// Register a zone component as singleton manually
    /// </summary>
    public static IServiceCollection AddZoneComponentSingleton<T>(this IServiceCollection services) where T : class, IZoneComponent
    {
        services.AddSingleton<T>();
        services.AddSingleton<IZoneComponent, T>();
        return services;
    }

    /// <summary>
    /// Register a ZoneViewComponent manually
    /// </summary>
    public static IServiceCollection AddZoneViewComponent<TModel, TComponent>(this IServiceCollection services) 
        where TComponent : ZoneViewComponent<TModel>, new() where TModel : IZone
    {
        services.AddScoped<TComponent>();
        services.AddScoped<IZoneComponent, TComponent>();
        return services;
    }

    /// <summary>
    /// Register a ZoneViewComponent with context manually
    /// </summary>
    public static IServiceCollection AddZoneViewComponent<TModel, TContext, TComponent>(this IServiceCollection services) 
        where TComponent : ZoneViewComponent<TModel, TContext>, new()
    {
        services.AddScoped<TComponent>();
        services.AddScoped<IZoneComponent, TComponent>();
        return services;
    }

    /// <summary>
    /// Register a zone component instance manually
    /// </summary>
    public static IServiceCollection AddZoneComponentInstance<T>(this IServiceCollection services, T instance) where T : class, IZoneComponent
    {
        services.AddSingleton<IZoneComponent>(instance);
        return services;
    }

    /// <summary>
    /// Startup filter that registers components in the registry
    /// </summary>
    private sealed class ZoneStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                var registry = app.ApplicationServices.GetRequiredService<IZoneRegistry>();
                
                // Get all registered zone components and add them to the registry
                var zoneComponents = app.ApplicationServices.GetServices<IZoneComponent>();
                foreach (var component in zoneComponents)
                {
                    registry.Add(component);
                }

                next(app);
            };
        }
    }
} 