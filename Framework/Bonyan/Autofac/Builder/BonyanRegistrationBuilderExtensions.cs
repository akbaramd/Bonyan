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
            IBonModuleAccessor bonModuleContainer,
            BonServiceRegistrationActionList registrationActionList)
    {
        var serviceType = registrationBuilder.RegistrationData.Services
            .OfType<IServiceWithType>()
            .FirstOrDefault()?.ServiceType;
        if (serviceType == null)
        {
            return registrationBuilder;
        }

        Type? implementationType = GetImplementationType(registrationBuilder);

        registrationBuilder = registrationBuilder.EnablePropertyInjection(bonModuleContainer, implementationType);
        registrationBuilder = registrationBuilder.InvokeRegistrationActions(registrationActionList, serviceType, implementationType);

        return registrationBuilder;
    }

    private static Type? GetImplementationType<TLimit, TActivatorData, TRegistrationStyle>(
        IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder)
    {
        var activatorData = registrationBuilder.ActivatorData;

        if (activatorData is ReflectionActivatorData reflectionActivatorData)
        {
            return reflectionActivatorData.ImplementationType;
        }
        else if (activatorData is SimpleActivatorData simpleActivatorData)
        {
            return simpleActivatorData.Activator.LimitType;
        }
        else
        {
            // Cannot get implementation type
            return null;
        }
    }

    private static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> InvokeRegistrationActions<TLimit, TActivatorData, TRegistrationStyle>(
        this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
        BonServiceRegistrationActionList registrationActionList,
        Type serviceType,
        Type? implementationType)
    {
        var serviceRegisteredArgs = new OnServiceRegisteredContext(serviceType, implementationType);

        foreach (var registrationAction in registrationActionList)
        {
            registrationAction.Invoke(serviceRegisteredArgs);
        }

        if (serviceRegisteredArgs.Interceptors.Any())
        {
            registrationBuilder = registrationBuilder.AddInterceptors(
                registrationActionList,
                serviceType,
                serviceRegisteredArgs.Interceptors
            );
        }

        return registrationBuilder;
    }

    private static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> EnablePropertyInjection<TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
            IBonModuleAccessor bonModuleContainer,
            Type? implementationType)
    {
        if (implementationType == null)
        {
            return registrationBuilder;
        }

        // Enable Property Injection only for types in an assembly containing a BonyanModule and without a DisablePropertyInjection attribute.
        if (bonModuleContainer.GetAllModules().Any(m => m.AllAssemblies.Contains(implementationType.Assembly)) &&
            implementationType.GetCustomAttributes(typeof(DisablePropertyInjectionAttribute), true).IsNullOrEmpty())
        {
            registrationBuilder = registrationBuilder.PropertiesAutowired(new BonyanPropertySelector(false));
        }

        return registrationBuilder;
    }

    private static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
        AddInterceptors<TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
            BonServiceRegistrationActionList bonServiceRegistrationActionList,
            Type serviceType,
            IEnumerable<Type> interceptors)
    {
        if (serviceType.IsInterface)
        {
            registrationBuilder = registrationBuilder.EnableInterfaceInterceptors();
        }
        else
        {
            if (bonServiceRegistrationActionList.IsClassInterceptorsDisabled)
            {
                return registrationBuilder;
            }

            if (registrationBuilder is IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle> concreteBuilder)
            {
                concreteBuilder.EnableClassInterceptors();
                // Cannot assign back to registrationBuilder due to type constraints
            }
            else
            {
                // Cannot enable class interceptors on non-concrete types
                return registrationBuilder;
            }
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
