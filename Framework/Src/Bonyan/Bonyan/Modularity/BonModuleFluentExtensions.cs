using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity.Abstractions;

/// <summary>
/// Fluent API extensions for service registration in modules.
/// Provides a chainable interface for registering services.
/// </summary>
public static class BonModuleFluentExtensions
{
    /// <summary>
    /// Adds a service with the specified lifetime.
    /// </summary>
    public static BonModule AddService<TService, TImplementation>(
        this BonModule module,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(module);
        ArgumentNullException.ThrowIfNull(module.Services);

        module.Services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
        return module;
    }

    /// <summary>
    /// Adds a singleton service.
    /// </summary>
    public static BonModule AddSingleton<TService, TImplementation>(this BonModule module)
        where TService : class
        where TImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(module);
        ArgumentNullException.ThrowIfNull(module.Services);

        module.Services.AddSingleton<TService, TImplementation>();
        return module;
    }

    /// <summary>
    /// Adds a singleton service instance.
    /// </summary>
    public static BonModule AddSingleton<TService>(this BonModule module, TService instance)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(module);
        ArgumentNullException.ThrowIfNull(module.Services);
        ArgumentNullException.ThrowIfNull(instance);

        module.Services.AddSingleton(instance);
        return module;
    }

    /// <summary>
    /// Adds a singleton service factory.
    /// </summary>
    public static BonModule AddSingleton<TService>(
        this BonModule module,
        Func<IServiceProvider, TService> factory)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(module);
        ArgumentNullException.ThrowIfNull(module.Services);
        ArgumentNullException.ThrowIfNull(factory);

        module.Services.AddSingleton(factory);
        return module;
    }

    /// <summary>
    /// Adds a scoped service.
    /// </summary>
    public static BonModule AddScoped<TService, TImplementation>(this BonModule module)
        where TService : class
        where TImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(module);
        ArgumentNullException.ThrowIfNull(module.Services);

        module.Services.AddScoped<TService, TImplementation>();
        return module;
    }

    /// <summary>
    /// Adds a scoped service factory.
    /// </summary>
    public static BonModule AddScoped<TService>(
        this BonModule module,
        Func<IServiceProvider, TService> factory)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(module);
        ArgumentNullException.ThrowIfNull(module.Services);
        ArgumentNullException.ThrowIfNull(factory);

        module.Services.AddScoped(factory);
        return module;
    }

    /// <summary>
    /// Adds a transient service.
    /// </summary>
    public static BonModule AddTransient<TService, TImplementation>(this BonModule module)
        where TService : class
        where TImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(module);
        ArgumentNullException.ThrowIfNull(module.Services);

        module.Services.AddTransient<TService, TImplementation>();
        return module;
    }

    /// <summary>
    /// Adds a transient service factory.
    /// </summary>
    public static BonModule AddTransient<TService>(
        this BonModule module,
        Func<IServiceProvider, TService> factory)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(module);
        ArgumentNullException.ThrowIfNull(module.Services);
        ArgumentNullException.ThrowIfNull(factory);

        module.Services.AddTransient(factory);
        return module;
    }

    /// <summary>
    /// Adds multiple services with the same implementation.
    /// </summary>
    public static BonModule AddAs<TImplementation>(
        this BonModule module,
        params Type[] serviceTypes)
        where TImplementation : class
    {
        ArgumentNullException.ThrowIfNull(module);
        ArgumentNullException.ThrowIfNull(module.Services);
        ArgumentNullException.ThrowIfNull(serviceTypes);

        foreach (var serviceType in serviceTypes)
        {
            module.Services.AddScoped(serviceType, typeof(TImplementation));
        }

        return module;
    }

    /// <summary>
    /// Decorates a service with a decorator pattern.
    /// </summary>
    public static BonModule Decorate<TService, TDecorator>(this BonModule module)
        where TService : class
        where TDecorator : class, TService
    {
        ArgumentNullException.ThrowIfNull(module);
        ArgumentNullException.ThrowIfNull(module.Services);

        // Get existing registration
        var descriptor = module.Services.FirstOrDefault(s => s.ServiceType == typeof(TService));
        if (descriptor == null)
        {
            throw new InvalidOperationException($"Service {typeof(TService).Name} is not registered. Register it before decorating.");
        }

        // Remove existing registration
        module.Services.Remove(descriptor);

        // Add decorator factory
        module.Services.Add(new ServiceDescriptor(
            typeof(TService),
            sp =>
            {
                var decorated = descriptor.ImplementationInstance 
                    ?? descriptor.ImplementationFactory?.Invoke(sp)
                    ?? ActivatorUtilities.CreateInstance(sp, descriptor.ImplementationType!);
                
                return ActivatorUtilities.CreateInstance<TDecorator>(sp, decorated);
            },
            descriptor.Lifetime));

        return module;
    }
}

