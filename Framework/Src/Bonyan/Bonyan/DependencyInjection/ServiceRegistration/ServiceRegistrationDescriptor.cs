using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Immutable descriptor for a single service registration (value object).
/// Defines lifetime, replace behavior, and which service types the implementation is registered as.
/// </summary>
public sealed class ServiceRegistrationDescriptor
{
    /// <summary>Service lifetime.</summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>When true, replace existing registration; when false, use TryAdd.</summary>
    public bool ReplaceExisting { get; }

    /// <summary>Service types to register (e.g. interfaces). Empty means register as implementation type only.</summary>
    public IReadOnlyList<Type> ServiceTypes { get; }

    public ServiceRegistrationDescriptor(
        ServiceLifetime lifetime,
        bool replaceExisting,
        IReadOnlyList<Type>? serviceTypes = null)
    {
        Lifetime = lifetime;
        ReplaceExisting = replaceExisting;
        ServiceTypes = serviceTypes ?? Array.Empty<Type>();
    }

    public static ServiceRegistrationDescriptor ForSelf(ServiceLifetime lifetime, bool replaceExisting = false)
        => new(lifetime, replaceExisting, Array.Empty<Type>());
}
