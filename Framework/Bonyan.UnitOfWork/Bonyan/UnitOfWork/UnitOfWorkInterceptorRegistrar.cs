using System.Reflection;
using Bonyan.DependencyInjection;
using Bonyan.DynamicProxy;

namespace Bonyan.UnitOfWork;

public static class UnitOfWorkInterceptorRegistrar
{
    public static void RegisterIfNeeded(IOnServiceRegistredContext context)
    {
        if (ShouldIntercept(context.ImplementationType))
        {
            context.Interceptors.TryAdd<BonUnitOfWorkInterceptor>();
        }
    }

    private static bool ShouldIntercept(Type type)
    {
        return !DynamicProxyIgnoreTypes.Contains(type) && BonUnitOfWorkHelper.IsUnitOfWorkType(type.GetTypeInfo());
    }
}
