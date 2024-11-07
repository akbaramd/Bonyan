
using Autofac;
using Bonyan.Castle.DynamicProxy;
using Bonyan.DependencyInjection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan;

public class BonMasterModule : BonModule
{

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.TryAddObjectAccessor<IServiceProvider>();
        context.Services.AddTransient(typeof(BonAsyncDeterminationInterceptor<>));
        context.Services.AddTransient<IBonCachedServiceProviderBase, BonLazyServiceProvider>();
        context.Services.AddTransient<IBonLazyServiceProvider, BonLazyServiceProvider>();
        
        return base.OnConfigureAsync(context);
    }
}