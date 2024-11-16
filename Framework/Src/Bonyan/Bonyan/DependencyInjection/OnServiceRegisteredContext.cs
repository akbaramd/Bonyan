using Bonyan.Collections;
using Bonyan.Core;
using JetBrains.Annotations;

namespace Bonyan.DependencyInjection;

public class OnServiceRegisteredContext : IOnServiceRegistredContext
{
  public virtual ITypeList<IBonInterceptor> Interceptors { get; }

  public virtual Type ServiceType { get; }

  public virtual Type ImplementationType { get; }

  public OnServiceRegisteredContext(Type serviceType, [NotNull] Type implementationType)
  {
    ServiceType = Check.NotNull(serviceType, nameof(serviceType));
    ImplementationType = Check.NotNull(implementationType, nameof(implementationType));
    Interceptors = new TypeList<IBonInterceptor>();
  }
}
