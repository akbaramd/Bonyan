using System.Reflection;
using Bonyan.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// ABP-style base for conventional registrars: only <see cref="CanRegister"/> and <see cref="GetRegistration"/> (no add logic).
/// Adding to the service collection is done by <see cref="ServiceCollectionConventionalRegistrarExtensions.AddAssembly"/>.
/// Override <see cref="GetCandidateTypes"/> to filter types (e.g. only types with attribute).
/// </summary>
public abstract class ConventionalRegistrarBase : IConventionalRegistrar
{
    /// <inheritdoc />
    public virtual IEnumerable<Type> GetCandidateTypes(Assembly assembly)
    {
        Type[] types;
        try
        {
            types = AssemblyHelper
                .GetAllTypes(assembly)
                .Where(t => t != null && t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
                .ToArray();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types!
                .Where(t => t != null && t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Select(t => t!)
                .ToArray();
        }

        return types;
    }

    /// <inheritdoc />
    public abstract bool CanRegister(Type type);

    /// <inheritdoc />
    public abstract ServiceRegistrationDescriptor? GetRegistration(Type type);

    /// <summary>When true, the type is not registered (e.g. has <see cref="DisableConventionalRegistrationAttribute"/>).</summary>
    protected virtual bool IsConventionalRegistrationDisabled(Type type)
    {
        return type.IsDefined(typeof(DisableConventionalRegistrationAttribute), true);
    }

    /// <summary>Lifetime for the type; null means do not register. From <see cref="BonServiceAttribute"/> or convention.</summary>
    protected virtual ServiceLifetime? GetLifetimeOrNull(Type type, BonServiceAttribute? attribute)
    {
        return attribute?.Lifetime ?? GetDefaultLifetimeOrNull(type);
    }

    /// <summary>Override to provide default lifetime by convention (e.g. by type name).</summary>
    protected virtual ServiceLifetime? GetDefaultLifetimeOrNull(Type type) => null;

    /// <summary>
    /// Service types to register (ABP-style).
    /// When <see cref="BonServiceAttribute.ServiceTypes"/> is set: register implementation + those types (resolve by interface or concrete).
    /// When null or empty: register only implementation type (no auto-added interfaces).
    /// </summary>
    protected virtual IReadOnlyList<Type> GetExposedServiceTypes(Type implementationType, BonServiceAttribute? attribute)
    {
        if (attribute?.ServiceTypes != null && attribute.ServiceTypes.Length > 0)
        {
            var serviceTypes = new List<Type> { implementationType };
            foreach (var st in attribute.ServiceTypes)
            {
                if (st != null && st.IsAssignableFrom(implementationType))
                    serviceTypes.Add(st);
            }
            return serviceTypes;
        }
        return new List<Type> { implementationType };
    }

    /// <summary>Returns implementation type (self) + all implemented interfaces suitable for DI. Used by convention (e.g. BonConventionRegistrar) when no attribute.</summary>
    protected static IReadOnlyList<Type> GetSelfAndImplementedInterfaces(Type implementationType)
    {
        var list = new List<Type> { implementationType };
        var interfaces = implementationType.GetInterfaces().Where(IsExposableServiceInterface);
        list.AddRange(interfaces);
        return list;
    }

    /// <summary>Whether the interface should be registered (exclude IDisposable, IAsyncDisposable, etc.).</summary>
    protected static bool IsExposableServiceInterface(Type interfaceType)
    {
        if (interfaceType == null || !interfaceType.IsInterface)
            return false;
        var name = interfaceType.Name;
        if (name == nameof(IDisposable) || name == nameof(IAsyncDisposable))
            return false;
        if (interfaceType.Assembly == typeof(object).Assembly)
            return false;
        return true;
    }

    /// <summary>Whether to replace existing registration for the same service type.</summary>
    protected virtual bool GetReplaceExisting(Type type, BonServiceAttribute? attribute)
    {
        return attribute?.ReplaceExisting ?? false;
    }

    protected virtual BonServiceAttribute? GetBonServiceAttributeOrNull(Type type)
    {
        return type.GetCustomAttribute<BonServiceAttribute>(true);
    }
}
