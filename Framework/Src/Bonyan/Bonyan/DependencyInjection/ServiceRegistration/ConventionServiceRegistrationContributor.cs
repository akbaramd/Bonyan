using System.Reflection;
using Bonyan.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Convention-based contributor: registers concrete types by naming convention (e.g. *Service, *Repository).
/// Only considers types that do NOT have <see cref="BonServiceAttribute"/> (attribute contributor takes precedence).
/// Lifetime is determined by <see cref="GetLifetimeForConvention"/>; override to customize.
/// </summary>
public class ConventionServiceRegistrationContributor : ServiceRegistrationContributorBase
{
    private readonly IReadOnlyList<ConventionRule> _conventions;

    public ConventionServiceRegistrationContributor()
        : this(DefaultConventions())
    {
    }

    public ConventionServiceRegistrationContributor(IReadOnlyList<ConventionRule> conventions)
    {
        _conventions = conventions ?? throw new ArgumentNullException(nameof(conventions));
    }

    /// <summary>Default conventions: *Service, *Repository, *Handler -> Scoped; *Factory -> Transient; *Provider -> Singleton.</summary>
    public static IReadOnlyList<ConventionRule> DefaultConventions()
    {
        return new[]
        {
            new ConventionRule(s => s.EndsWith("Service", StringComparison.Ordinal), ServiceLifetime.Scoped),
            new ConventionRule(s => s.EndsWith("Repository", StringComparison.Ordinal), ServiceLifetime.Scoped),
            new ConventionRule(s => s.EndsWith("Handler", StringComparison.Ordinal), ServiceLifetime.Scoped),
            new ConventionRule(s => s.EndsWith("Factory", StringComparison.Ordinal), ServiceLifetime.Transient),
            new ConventionRule(s => s.EndsWith("Provider", StringComparison.Ordinal), ServiceLifetime.Singleton),
        };
    }

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
                if (type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition)
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

        if (type.GetCustomAttribute<BonServiceAttribute>(false) != null)
            return false;

        return GetLifetimeForConvention(type, context) != null;
    }

    /// <summary>
    /// Returns the lifetime for the type based on conventions, or null to skip.
    /// Override to customize (e.g. use context.GetData for configuration).
    /// </summary>
    protected virtual ServiceLifetime? GetLifetimeForConvention(Type type, IServiceRegistrationContext context)
    {
        var name = type.Name;
        foreach (var rule in _conventions)
        {
            if (rule.Predicate(name))
                return rule.Lifetime;
        }
        return null;
    }

    public override ServiceRegistrationDescriptor? GetRegistration(Type type, IServiceRegistrationContext context)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(context);

        var lifetime = GetLifetimeForConvention(type, context);
        if (lifetime == null)
            return null;

        return ServiceRegistrationDescriptor.ForSelf(lifetime.Value, replaceExisting: false);
    }
}
