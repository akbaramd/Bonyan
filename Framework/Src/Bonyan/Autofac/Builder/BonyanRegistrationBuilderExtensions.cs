using System.Reflection;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Extras.DynamicProxy;
using Bonyan.Autofac;
using Bonyan.Castle.DynamicProxy;
using Bonyan.DependencyInjection;
using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;

public static class AbpRegistrationBuilderExtensions
{
    public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> ConfigureAbpConventions<TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
            ServiceDescriptor serviceDescriptor,
            IModuleContainer moduleContainer,
            BonServiceRegistrationActionList registrationActionList,
            BonServiceActivatedActionList activatedActionList)
        where TActivatorData : ReflectionActivatorData
    {
        registrationBuilder = registrationBuilder.InvokeActivatedActions(activatedActionList, serviceDescriptor);

        var serviceType = registrationBuilder.RegistrationData.Services.OfType<IServiceWithType>().FirstOrDefault()?.ServiceType;
        if (serviceType == null)
        {
            return registrationBuilder;
        }

        var implementationType = registrationBuilder.ActivatorData.ImplementationType;
        if (implementationType == null)
        {
            return registrationBuilder;
        }

        registrationBuilder = registrationBuilder.EnablePropertyInjection(moduleContainer, implementationType);
        registrationBuilder = registrationBuilder.InvokeRegistrationActions(registrationActionList, serviceType, implementationType, serviceDescriptor.ServiceKey);

        return registrationBuilder;
    }

    private static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> InvokeActivatedActions<TLimit, TActivatorData, TRegistrationStyle>(
        this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
        BonServiceActivatedActionList activatedActionList,
        ServiceDescriptor serviceDescriptor)
        where TActivatorData : ReflectionActivatorData
    {
        var actions = activatedActionList.GetActions(serviceDescriptor);
        if (actions.Any())
        {
            registrationBuilder.OnActivated(context =>
            {
                var serviceActivatedContext = new OnServiceActivatedContext(context.Instance!);
                foreach (var action in actions)
                {
                    action.Invoke(serviceActivatedContext);
                }
            });
        }

        return registrationBuilder;
    }

    private static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> InvokeRegistrationActions<TLimit, TActivatorData, TRegistrationStyle>(
        this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
        BonServiceRegistrationActionList registrationActionList,
        Type serviceType,
        Type implementationType,
        object? serviceKey = null)
        where TActivatorData : ReflectionActivatorData
    {
        var serviceRegistredArgs = new OnServiceRegisteredContext(serviceType, implementationType, serviceKey);

        foreach (var registrationAction in registrationActionList)
        {
            registrationAction.Invoke(serviceRegistredArgs);
        }

        if (serviceRegistredArgs.Interceptors.Any())
        {
            var disableAbpFeaturesAttribute = serviceRegistredArgs.ImplementationType.GetCustomAttribute<DisableBonFeaturesAttribute>(true);
            if (disableAbpFeaturesAttribute == null || !disableAbpFeaturesAttribute.DisableInterceptors)
            {
                registrationBuilder = registrationBuilder.AddInterceptors(
                    registrationActionList,
                    serviceType,
                    serviceRegistredArgs.Interceptors
                );
            }
        }

        return registrationBuilder;
    }

    private static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> EnablePropertyInjection<TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
            IModuleContainer moduleContainer,
            Type implementationType)
        where TActivatorData : ReflectionActivatorData
    {
        // Enable Property Injection only for types in an assembly containing an AbpModule and without a DisablePropertyInjection attribute on class or properties.
        if (moduleContainer.Modules.Any(m => m.AllAssemblies.Contains(implementationType.Assembly)) &&
            implementationType.GetCustomAttributes(typeof(DisablePropertyInjectionAttribute), true).IsNullOrEmpty())
        {
            registrationBuilder = registrationBuilder.PropertiesAutowired(new BonyanPropertySelector(false));
        }

        return registrationBuilder;
    }

    private static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
        AddInterceptors<TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
            BonServiceRegistrationActionList serviceRegistrationActionList,
            Type serviceType,
            IEnumerable<Type> interceptors)
        where TActivatorData : ReflectionActivatorData
    {
        if (serviceType.IsInterface)
        {
            registrationBuilder = registrationBuilder.EnableInterfaceInterceptors();
        }
        else
        {
            if (serviceRegistrationActionList.IsClassInterceptorsDisabled ||
                serviceRegistrationActionList.DisabledClassInterceptorsSelectors.Any(selector => selector.Predicate(serviceType)))
            {
                return registrationBuilder;
            }

            (registrationBuilder as IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle>)?.EnableClassInterceptors();
        }

        foreach (var interceptor in interceptors)
        {
            registrationBuilder.InterceptedBy(
                typeof(BonAsyncDeterminationInterceptor<>).MakeGenericType(interceptor)
            );
        }

        return registrationBuilder;
    }
}