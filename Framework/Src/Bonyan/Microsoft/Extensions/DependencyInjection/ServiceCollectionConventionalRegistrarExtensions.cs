using System.Reflection;
using Bonyan.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for conventional service registration (ABP-style).
/// Get all registrars, add assembly to registrars, and add types to the service collection in one place.
/// </summary>
public static class ServiceCollectionConventionalRegistrarExtensions
{
    /// <summary>
    /// Gets the list of conventional registrars, or null if none have been added yet.
    /// </summary>
    public static ConventionalRegistrarList? GetConventionalRegistrarList(this IServiceCollection services)
    {
        return services.GetObjectOrNull<ConventionalRegistrarList>();
    }

    /// <summary>
    /// Gets all conventional registrars (creates the list with defaults if not yet created).
    /// Use with <see cref="AddAssembly"/> to run registration for an assembly (ABP-style).
    /// </summary>
    public static ConventionalRegistrarList GetConventionalRegistrars(this IServiceCollection services)
    {
        return services.GetOrCreateConventionalRegistrarList();
    }

    /// <summary>
    /// Gets or creates the list of conventional registrars. If created, adds defaults:
    /// <see cref="BonAttributeConventionalRegistrar"/> ([BonService]) and <see cref="BonConventionRegistrar"/> (*Service, *Repository, etc.).
    /// </summary>
    public static ConventionalRegistrarList GetOrCreateConventionalRegistrarList(this IServiceCollection services)
    {
        var list = services.GetObjectOrNull<ConventionalRegistrarList>();
        if (list == null)
        {
            list = new ConventionalRegistrarList();
            list.Add(new BonAttributeConventionalRegistrar());
            list.Add(new BonConventionRegistrar());
            services.AddObjectAccessor(list);
        }
        return list;
    }

    /// <summary>
    /// Adds a conventional registrar. Call from module's OnConfigureAsync (e.g. context.Services.AddConventionalRegistrar(...)).
    /// </summary>
    public static IServiceCollection AddConventionalRegistrar(this IServiceCollection services, IConventionalRegistrar registrar)
    {
        ArgumentNullException.ThrowIfNull(registrar);
        services.GetOrCreateConventionalRegistrarList().Add(registrar);
        return services;
    }

    /// <summary>
    /// Runs all conventional registrars for the given assembly and adds matching types to the service collection (ABP-style).
    /// Gets all registrars via <see cref="GetConventionalRegistrars"/>, then for each registrar: GetCandidateTypes(assembly), CanRegister(type), GetRegistration(type), then adds to services.
    /// </summary>
    public static IServiceCollection AddAssembly(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        var registrars = services.GetConventionalRegistrars();
        foreach (var registrar in registrars)
        {
            foreach (var type in registrar.GetCandidateTypes(assembly))
            {
                if (type == null || !registrar.CanRegister(type))
                    continue;

                var descriptor = registrar.GetRegistration(type);
                if (descriptor == null)
                    continue;

                AddToServices(services, type, descriptor);
            }
        }

        return services;
    }

    /// <summary>
    /// Runs conventional registration for the assembly containing <typeparamref name="T"/>.
    /// Example: services.AddAssemblyOf&lt;IBonModularityApplication&gt;() or services.AddAssemblyOf&lt;MyModule&gt;().
    /// </summary>
    public static IServiceCollection AddAssemblyOf<T>(this IServiceCollection services)
    {
        return services.AddAssembly(typeof(T).Assembly);
    }

    /// <summary>
    /// Runs conventional registration for the assembly of the given type (e.g. services.AddAssembly(typeof(MyModule))).
    /// </summary>
    public static IServiceCollection AddAssembly(this IServiceCollection services, Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return services.AddAssembly(type.Assembly);
    }

    private static void AddToServices(IServiceCollection services, Type implementationType, ServiceRegistrationDescriptor descriptor)
    {
        var serviceTypes = descriptor.ServiceTypes.Count > 0
            ? descriptor.ServiceTypes
            : new[] { implementationType };

        foreach (var serviceType in serviceTypes)
        {
            if (serviceType == null || !serviceType.IsAssignableFrom(implementationType))
                continue;

            var sd = new ServiceDescriptor(serviceType, implementationType, descriptor.Lifetime);
            if (descriptor.ReplaceExisting)
            {
                RemoveExisting(services, serviceType);
                services.Add(sd);
            }
            else
            {
                services.TryAdd(sd);
            }
        }
    }

    private static void RemoveExisting(IServiceCollection services, Type serviceType)
    {
        for (var i = services.Count - 1; i >= 0; i--)
        {
            if (services[i].ServiceType == serviceType)
                services.RemoveAt(i);
        }
    }
}
