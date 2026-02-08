using System.Reflection;
using Bonyan.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Conventional registrar that only considers types marked with <see cref="BonServiceAttribute"/> (or derived).
/// GetCandidateTypes filters to types that have the attribute; adding to the service collection is handled by the extension.
/// </summary>
public class BonAttributeConventionalRegistrar : ConventionalRegistrarBase
{
    /// <inheritdoc />
    public override IEnumerable<Type> GetCandidateTypes(Assembly assembly)
    {
        IReadOnlyList<Type> types;
        try
        {
            types = AssemblyHelper.GetAllTypes(assembly);
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types!;
        }

        foreach (var type in types)
        {
            if (type == null || !type.IsClass || type.IsAbstract || type.IsGenericTypeDefinition)
                continue;
            if (type.GetCustomAttribute<BonServiceAttribute>(false) != null)
                yield return type;
        }
    }

    /// <inheritdoc />
    public override bool CanRegister(Type type)
    {
        if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
            return false;
        if (IsConventionalRegistrationDisabled(type))
            return false;
        return GetBonServiceAttributeOrNull(type) != null;
    }

    /// <inheritdoc />
    public override ServiceRegistrationDescriptor? GetRegistration(Type type)
    {
        var attribute = GetBonServiceAttributeOrNull(type);
        if (attribute == null)
            return null;

        var lifetime = GetLifetimeOrNull(type, attribute);
        if (lifetime == null)
            return null;

        var serviceTypes = GetExposedServiceTypes(type, attribute);
        var replaceExisting = GetReplaceExisting(type, attribute);

        return new ServiceRegistrationDescriptor(lifetime.Value, replaceExisting, serviceTypes);
    }
}
