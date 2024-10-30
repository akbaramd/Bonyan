
using Bonyan.Castle.DynamicProxy;
using Bonyan.DependencyInjection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

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