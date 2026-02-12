using Bonyan.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionRegistrationActionExtensions
{
    // OnRegistered

    public static void OnRegistered(this IServiceCollection services, Action<IOnServiceRegisteredContext> registrationAction)
    {
        GetOrCreateRegistrationActionList(services).Add(registrationAction);
    }

    public static BonServiceRegistrationActionList GetRegistrationActionList(this IServiceCollection services)
    {
        return GetOrCreateRegistrationActionList(services);
    }

    private static BonServiceRegistrationActionList GetOrCreateRegistrationActionList(IServiceCollection services)
    {
        var actionList = services.GetSingletonInstanceOrNull<IBonObjectAccessor<BonServiceRegistrationActionList>>()?.Value;
        if (actionList == null)
        {
            actionList = new BonServiceRegistrationActionList();
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

    // OnActivated

    public static void OnActivated<T>(this IServiceCollection services, Action<IOnServiceActivatedContext> action)
    {
        GetOrCreateActivatedActionList(services).Add<T>(action);
    }

    public static BonServiceActivatedActionList GetActivatedActionList(this IServiceCollection services)
    {
        return GetOrCreateActivatedActionList(services);
    }

    private static BonServiceActivatedActionList GetOrCreateActivatedActionList(IServiceCollection services)
    {
        var actionList = services.GetSingletonInstanceOrNull<IBonObjectAccessor<BonServiceActivatedActionList>>()?.Value;
        if (actionList == null)
        {
            actionList = new BonServiceActivatedActionList();
            services.AddObjectAccessor(actionList);
        }

        return actionList;
    }

    // OnExposing

    public static void OnExposing(this IServiceCollection services, Action<IOnServiceExposingContext> exposeAction)
    {
        GetOrCreateExposingList(services).Add(exposeAction);
    }

    public static BonServiceExposingActionList GetExposingActionList(this IServiceCollection services)
    {
        return GetOrCreateExposingList(services);
    }

    private static BonServiceExposingActionList GetOrCreateExposingList(IServiceCollection services)
    {
        var actionList = services.GetSingletonInstanceOrNull<IBonObjectAccessor<BonServiceExposingActionList>>()?.Value;
        if (actionList == null)
        {
            actionList = new BonServiceExposingActionList();
            services.AddObjectAccessor(actionList);
        }

        return actionList;
    }
}
