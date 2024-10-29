using Bonyan.Castle.DynamicProxy;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Castle;

[DependOn(typeof(BonyanCastleCoreModule))]
public class BonyanCastleCoreModule : Module
{
    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
        context.Services.AddTransient(typeof(AbpAsyncDeterminationInterceptor<>));
        
        return base.OnConfigureAsync(context);
    }
}
