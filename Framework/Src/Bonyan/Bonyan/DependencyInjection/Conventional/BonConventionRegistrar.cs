using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Conventional registrar that only considers types by naming convention (e.g. *Service, *Repository).
/// Skips types with <see cref="BonServiceAttribute"/> (handled by <see cref="BonAttributeConventionalRegistrar"/>).
/// Adding to the service collection is handled by the extension.
/// </summary>
public class BonConventionRegistrar : ConventionalRegistrarBase
{
    private readonly IReadOnlyList<ConventionRule> _conventions;

    public BonConventionRegistrar()
        : this(DefaultConventions())
    {
    }

    public BonConventionRegistrar(IReadOnlyList<ConventionRule> conventions)
    {
        _conventions = conventions ?? throw new ArgumentNullException(nameof(conventions));
    }

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

    /// <inheritdoc />
    public override bool CanRegister(Type type)
    {
        if (IsConventionalRegistrationDisabled(type))
            return false;
        if (GetBonServiceAttributeOrNull(type) != null)
            return false;
        return GetDefaultLifetimeOrNull(type) != null;
    }

    /// <inheritdoc />
    public override ServiceRegistrationDescriptor? GetRegistration(Type type)
    {
        var lifetime = GetDefaultLifetimeOrNull(type);
        if (lifetime == null)
            return null;

        var serviceTypes = GetSelfAndImplementedInterfaces(type);
        return new ServiceRegistrationDescriptor(lifetime.Value, replaceExisting: false, serviceTypes);
    }

    protected override ServiceLifetime? GetDefaultLifetimeOrNull(Type type)
    {
        var name = type.Name;
        foreach (var rule in _conventions)
        {
            if (rule.Predicate(name))
                return rule.Lifetime;
        }
        return null;
    }
}
