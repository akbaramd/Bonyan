using Bonyan.Collections;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Context passed when a service is registered; used to add interceptors or customize registration.
/// </summary>
public interface IOnServiceRegisteredContext
{
    /// <summary>Service type (e.g. interface) being registered.</summary>
    Type ServiceType { get; }

    /// <summary>Implementation type.</summary>
    Type ImplementationType { get; }

    /// <summary>List of interceptors to apply to this service.</summary>
    ITypeList<IBonInterceptor> Interceptors { get; }
}
