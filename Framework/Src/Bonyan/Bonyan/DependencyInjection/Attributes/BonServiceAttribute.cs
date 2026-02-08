using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Marks a class for automatic registration in the dependency injection container (ABP-style).
/// Controls lifetime, replace behavior, and which service types the implementation is registered as.
/// </summary>
/// <remarks>
/// <para><b>When <see cref="ServiceTypes"/> is set:</b> Registers the implementation as those types and as the implementation type (so you can resolve by interface or by concrete type).</para>
/// <para><b>When <see cref="ServiceTypes"/> is null or empty:</b> Registers only the implementation type. Implemented interfaces are not auto-registered (ABP behavior).</para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class BonServiceAttribute : Attribute
{
    /// <summary>
    /// Service lifetime: Transient, Scoped, or Singleton.
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// When true, uses Add (replaces any existing registration for the same service type).
    /// When false, uses TryAdd (keeps first registration; does not replace).
    /// </summary>
    public bool ReplaceExisting { get; }

    /// <summary>
    /// Optional service types (e.g. interfaces) to register this implementation as.
    /// If null or empty, only the implementation type is registered (no automatic interface registration, like ABP).
    /// </summary>
    public Type[]? ServiceTypes { get; }

    /// <summary>
    /// Registers the class with the given lifetime and replace behavior.
    /// </summary>
    /// <param name="lifetime">Transient, Scoped, or Singleton.</param>
    /// <param name="replaceExisting">True to replace existing registration; false to skip if already registered.</param>
    /// <param name="serviceTypes">Optional interfaces/base types to register as. If null, registers as self.</param>
    public BonServiceAttribute(
        ServiceLifetime lifetime,
        bool replaceExisting = false,
        params Type[]? serviceTypes)
    {
        Lifetime = lifetime;
        ReplaceExisting = replaceExisting;
        ServiceTypes = serviceTypes;
    }
}
