using Bonyan.Collections;
using Bonyan.Core;
using JetBrains.Annotations;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Default implementation of <see cref="IOnServiceRegisteredContext"/>.
/// </summary>
public class OnServiceRegisteredContext : IOnServiceRegisteredContext
{
    /// <inheritdoc />
    public virtual Type ServiceType { get; }

    /// <inheritdoc />
    public virtual Type ImplementationType { get; }

    /// <inheritdoc />
    public virtual ITypeList<IBonInterceptor> Interceptors { get; }

    public OnServiceRegisteredContext(Type serviceType, [NotNull] Type implementationType)
    {
        ServiceType = Check.NotNull(serviceType, nameof(serviceType));
        ImplementationType = Check.NotNull(implementationType, nameof(implementationType));
        Interceptors = new TypeList<IBonInterceptor>();
    }
}
