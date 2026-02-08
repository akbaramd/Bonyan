using System.Reflection;
using Bonyan.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Default contributor: discovers types with <see cref="BonServiceAttribute"/> (or derived) and registers them.
/// Lifetime and replace behavior come from the attribute; CanRegister can be overridden for filtering.
/// </summary>
public class BonAttributeServiceRegistrationContributor : ServiceRegistrationContributorBase
{
    public override IEnumerable<Type> GetCandidateTypes(IServiceRegistrationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        foreach (var assembly in context.Assemblies)
        {
            IReadOnlyList<Type> types;
            try
            {
                types = AssemblyHelper.GetAllTypes(assembly);
            }
            catch
            {
                continue;
            }

            foreach (var type in types)
            {
                if (type == null)
                    continue;
                if (type.GetCustomAttribute<BonServiceAttribute>(false) != null)
                    yield return type;
            }
        }
    }

    public override bool CanRegister(Type type, IServiceRegistrationContext context)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(context);

        if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
            return false;

        return type.GetCustomAttribute<BonServiceAttribute>(false) != null;
    }

    public override ServiceRegistrationDescriptor? GetRegistration(Type implementationType, IServiceRegistrationContext context)
    {
        ArgumentNullException.ThrowIfNull(implementationType);
        ArgumentNullException.ThrowIfNull(context);

        var attr = implementationType.GetCustomAttribute<BonServiceAttribute>(false);
        if (attr == null)
            return null;

        // ABP-style: when ServiceTypes set → register implementation + those types; when not set → implementation only
        IReadOnlyList<Type> descriptorServiceTypes;
        if (attr.ServiceTypes != null && attr.ServiceTypes.Length > 0)
        {
            var list = new List<Type> { implementationType };
            foreach (var st in attr.ServiceTypes)
            {
                if (st != null && st.IsAssignableFrom(implementationType))
                    list.Add(st);
            }
            descriptorServiceTypes = list;
        }
        else
        {
            // Empty → base Contribute uses implementationType only (no auto interfaces)
            descriptorServiceTypes = Array.Empty<Type>();
        }

        return new ServiceRegistrationDescriptor(attr.Lifetime, attr.ReplaceExisting, descriptorServiceTypes);
    }
}
