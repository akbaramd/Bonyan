using Bonyan.Collections;

namespace Bonyan.DependencyInjection;


public interface IOnServiceRegistredContext
{
  ITypeList<IBonInterceptor> Interceptors { get; }

  Type ImplementationType { get; }
}