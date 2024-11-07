using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac.Builder;
using Bonyan.Exceptions;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Autofac.Extensions.DependencyInjection;

public static class AutofacRegistration
{
    public static void Populate(
        this ContainerBuilder builder,
        IServiceCollection services)
    {
        Populate(builder, services, null);
    }

    public static void Populate(
        this ContainerBuilder builder,
        IServiceCollection services,
        object? lifetimeScopeTagForSingletons)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        builder.RegisterType<AutofacServiceProvider>()
            .As<IServiceProvider>()
            .As<IServiceProviderIsService>()
            .ExternallyOwned();

        var autofacServiceScopeFactory = typeof(AutofacServiceProvider).Assembly.GetType("Autofac.Extensions.DependencyInjection.AutofacServiceScopeFactory");
        if (autofacServiceScopeFactory == null)
        {
            throw new BusinessException(
                code:$"{nameof(AutofacRegistration)}:{nameof(Populate)}",
                message:"Unable get type of Autofac.Extensions.DependencyInjection.AutofacServiceScopeFactory!");
        }

        builder
            .RegisterType(autofacServiceScopeFactory)
            .As<IServiceScopeFactory>()
            .SingleInstance();

        Register(builder, services, lifetimeScopeTagForSingletons);
    }

    private static IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> ConfigureLifecycle<TActivatorData, TRegistrationStyle>(
        this IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registrationBuilder,
        ServiceLifetime lifecycleKind,
        object? lifetimeScopeTagForSingleton)
    {
        switch (lifecycleKind)
        {
            case ServiceLifetime.Singleton:
                if (lifetimeScopeTagForSingleton == null)
                {
                    registrationBuilder.SingleInstance();
                }
                else
                {
                    registrationBuilder.InstancePerMatchingLifetimeScope(lifetimeScopeTagForSingleton);
                }
                break;
            case ServiceLifetime.Scoped:
                registrationBuilder.InstancePerLifetimeScope();
                break;
            case ServiceLifetime.Transient:
                registrationBuilder.InstancePerDependency();
                break;
        }
        return registrationBuilder;
    }

    [SuppressMessage("CA2000", "CA2000", Justification = "Registrations are disposed when the container is disposed.")]
    private static void Register(
        ContainerBuilder builder,
        IServiceCollection services,
        object? lifetimeScopeTagForSingletons)
    {
        var moduleContainer = services.GetSingletonInstance<IBonModuleAccessor>();

        foreach (var module in moduleContainer.GetAllModules().Select(x=>x.Instance))
        {
            if (module != null) builder.RegisterModule(module);
        }
        
        var registrationActionList = services.GetRegistrationActionList();

        foreach (var descriptor in services)
        {
            if (descriptor.ImplementationType != null)
            {
                var serviceTypeInfo = descriptor.ServiceType.GetTypeInfo();
                if (serviceTypeInfo.IsGenericTypeDefinition)
                {
                    builder
                        .RegisterGeneric(descriptor.ImplementationType)
                        .As(descriptor.ServiceType)
                        .ConfigureLifecycle(descriptor.Lifetime, lifetimeScopeTagForSingletons)
                        .ConfigureBonyanConventions(moduleContainer, registrationActionList);
                }
                else
                {
                    builder
                        .RegisterType(descriptor.ImplementationType)
                        .As(descriptor.ServiceType)
                        .ConfigureLifecycle(descriptor.Lifetime, lifetimeScopeTagForSingletons)
                        .ConfigureBonyanConventions(moduleContainer, registrationActionList);
                }
            }
            else if (descriptor.ImplementationFactory != null)
            {
                var registrationBuilder = RegistrationBuilder.ForDelegate(descriptor.ServiceType, (context, parameters) =>
                    {
                        var serviceProvider = context.Resolve<IServiceProvider>();
                        return descriptor.ImplementationFactory(serviceProvider);
                    })
                    .As(descriptor.ServiceType)
                    .ConfigureLifecycle(descriptor.Lifetime, lifetimeScopeTagForSingletons)
                    .ConfigureBonyanConventions(moduleContainer, registrationActionList);

                var registration = registrationBuilder.CreateRegistration();
                builder.RegisterComponent(registration);
            }
            else
            {
                builder
                    .RegisterInstance(descriptor.ImplementationInstance!)
                    .As(descriptor.ServiceType)
                    .ConfigureLifecycle(descriptor.Lifetime, null)
                    .ConfigureBonyanConventions(moduleContainer, registrationActionList);
            }
        }
    }
}
