using Bonyan.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft;

public static class ServiceCollectionRegistrationActionExtensions
{
    // OnRegistered

    public static void OnRegistered(this IServiceCollection services, Action<IOnServiceRegistredContext> registrationAction)
    {
        GetOrCreateRegistrationActionList(services).Add(registrationAction);
    }

    public static ServiceRegistrationActionList GetRegistrationActionList(this IServiceCollection services)
    {
        return GetOrCreateRegistrationActionList(services);
    }

    private static ServiceRegistrationActionList GetOrCreateRegistrationActionList(IServiceCollection services)
    {
        var actionList = services.GetSingletonInstanceOrNull<IObjectAccessor<ServiceRegistrationActionList>>()?.Value;
        if (actionList == null)
        {
            actionList = new ServiceRegistrationActionList();
            services.AddObjectAccessor(actionList);
        }

        return actionList;
    }

    public static void DisableBonyanClassInterceptors(this IServiceCollection services)
    {
        GetOrCreateRegistrationActionList(services).IsClassInterceptorsDisabled = true;
    }

    public static bool IsBonyanClassInterceptorsDisabled(this IServiceCollection services)
    {
        return GetOrCreateRegistrationActionList(services).IsClassInterceptorsDisabled;
    }

    // OnExposing

 
}
