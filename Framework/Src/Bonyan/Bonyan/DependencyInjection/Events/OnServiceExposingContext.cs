using Bonyan.Core;
using JetBrains.Annotations;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Default implementation of <see cref="IOnServiceExposingContext"/>.
/// </summary>
public class OnServiceExposingContext : IOnServiceExposingContext
{
    /// <inheritdoc />
    public Type ImplementationType { get; }

    /// <inheritdoc />
    public List<Type> ExposedTypes { get; }

    public OnServiceExposingContext([NotNull] Type implementationType, List<Type> exposedTypes)
    {
        ImplementationType = Check.NotNull(implementationType, nameof(implementationType));
        ExposedTypes = Check.NotNull(exposedTypes, nameof(exposedTypes));
    }
}
