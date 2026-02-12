using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bonyan.DependencyInjection;
using Bonyan.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Volo.Abp.Autofac;

[BonService(ServiceLifetime.Transient,serviceTypes:typeof(IInjectPropertiesService))]
public class AutoFacInjectPropertiesService : IInjectPropertiesService
{
    protected IServiceProvider ServiceProvider { get; }

    public AutoFacInjectPropertiesService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public virtual TService InjectProperties<TService>(TService instance) where TService : notnull
    {
        return InjectProperties(instance, false);
    }

    public virtual TService InjectUnsetProperties<TService>(TService instance) where TService : notnull
    {
        return InjectProperties(instance, true);
    }

    protected virtual TService InjectProperties<TService>(TService instance, bool onlyForUnsetProperties)
        where TService : notnull
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        if (ServiceProvider is not AutofacServiceProvider)
        {
            throw new BonException($"The {nameof(ServiceProvider)} must be an instance of {nameof(AutofacServiceProvider)}!");
        }

        return onlyForUnsetProperties
            ? ServiceProvider.As<AutofacServiceProvider>().LifetimeScope.InjectUnsetProperties(instance)
            : ServiceProvider.As<AutofacServiceProvider>().LifetimeScope.InjectProperties(instance);
    }
}