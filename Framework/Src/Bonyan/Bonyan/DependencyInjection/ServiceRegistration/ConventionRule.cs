using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Rule for convention-based registration: predicate on type name and lifetime.
/// </summary>
public sealed class ConventionRule
{
    /// <summary>Predicate on type name (e.g. name => name.EndsWith("Service")).</summary>
    public Func<string, bool> Predicate { get; }

    /// <summary>Service lifetime when the predicate matches.</summary>
    public ServiceLifetime Lifetime { get; }

    public ConventionRule(Func<string, bool> predicate, ServiceLifetime lifetime)
    {
        Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        Lifetime = lifetime;
    }
}
