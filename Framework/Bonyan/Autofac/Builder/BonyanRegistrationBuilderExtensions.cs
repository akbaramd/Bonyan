using Autofac.Core;
using Autofac.Extras.DynamicProxy;
using Bonyan.Autofac;
using Bonyan.Castle.DynamicProxy;
using Bonyan.DependencyInjection;
using Bonyan.Modularity.Abstractions;

namespace Autofac.Builder;

public static class BonyanRegistrationBuilderExtensions
{
    public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> ConfigureBonyanConventions<TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
            IModuleAccessor moduleContainer,
            ServiceRegistrationActionList registrationActionList)
        where TActivatorData : ReflectionActivatorData
    {
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
        registrationBuilder = registrationBuilder.InvokeRegistrationActions(registrationActionList, serviceType, implementationType);

        return registrationBuilder;
    }

    private static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> InvokeRegistrationActions<TLimit, TActivatorData, TRegistrationStyle>(
        this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
        ServiceRegistrationActionList registrationActionList,
        Type serviceType,
        Type implementationType)
        where TActivatorData : ReflectionActivatorData
    {
        var serviceRegistredArgs = new OnServiceRegisteredContext(serviceType, implementationType);

        foreach (var registrationAction in registrationActionList)
        {
            registrationAction.Invoke(serviceRegistredArgs);
        }

        if (serviceRegistredArgs.Interceptors.Any() )
        {
            registrationBuilder = registrationBuilder.AddInterceptors(
                registrationActionList,
                serviceType,
                serviceRegistredArgs.Interceptors
            );
        }

        return registrationBuilder;
    }

    private static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> EnablePropertyInjection<TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
            IModuleAccessor moduleContainer,
            Type implementationType)
        where TActivatorData : ReflectionActivatorData
    {
        // Enable Property Injection only for types in an assembly containing an BonyanModule and without a DisablePropertyInjection attribute on class or properties.
        if (moduleContainer.GetAllModules().Any(m => m.AllAssemblies.Contains(implementationType.Assembly)) &&
            implementationType.GetCustomAttributes(typeof(DisablePropertyInjectionAttribute), true).IsNullOrEmpty())
        {
            registrationBuilder = registrationBuilder.PropertiesAutowired(new BonyanPropertySelector(false));
        }

        return registrationBuilder;
    }

    private static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
        AddInterceptors<TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
            ServiceRegistrationActionList serviceRegistrationActionList,
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
            if (serviceRegistrationActionList.IsClassInterceptorsDisabled)
            {
                return registrationBuilder;
            }

            (registrationBuilder as IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle>)?.EnableClassInterceptors();
        }

        foreach (var interceptor in interceptors)
        {
            registrationBuilder.InterceptedBy(
                typeof(BonyanAsyncDeterminationInterceptor<>).MakeGenericType(interceptor)
            );
        }

        return registrationBuilder;
    }
}
