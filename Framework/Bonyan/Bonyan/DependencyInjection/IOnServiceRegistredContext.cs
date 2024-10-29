using Bonyan.Collections;

namespace Bonyan.DependencyInjection;


public interface IOnServiceRegistredContext
{
  ITypeList<IBonyanInterceptor> Interceptors { get; }

  Type ImplementationType { get; }
}