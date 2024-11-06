
using Autofac;
using Bonyan.Castle.DynamicProxy;
using Bonyan.DependencyInjection;
using Bonyan.Modularity;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;
using Module = Bonyan.Modularity.Abstractions.Module;

namespace Bonyan;

public class BonyanModule : Module
{

    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
        context.Services.TryAddObjectAccessor<IServiceProvider>();
        context.Services.AddTransient(typeof(BonyanAsyncDeterminationInterceptor<>));
        context.Services.AddTransient<ICachedServiceProviderBase, BonyanLazyServiceProvider>();
        context.Services.AddTransient<IBonyanLazyServiceProvider, BonyanLazyServiceProvider>();
        
        return base.OnConfigureAsync(context);
    }
}