using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.UnitOfWork;

public class BonyanUnitOfWorkModule : Module
{
    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<UnitOfWorkInterceptor>();
        context.Services.AddTransient<IUnitOfWork, UnitOfWork>();
        context.Services.AddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
        
        context.Services.AddSingleton<IUnitOfWorkTransactionBehaviourProvider, NullUnitOfWorkTransactionBehaviourProvider>();
        context.Services.AddSingleton<IUnitOfWorkEventPublisher, NullUnitOfWorkEventPublisher>();
        context.Services.AddSingleton<IAmbientUnitOfWork, AmbientUnitOfWork>();
        context.Services.AddSingleton<IUnitOfWorkAccessor, AmbientUnitOfWork>();
        
        context.Services.OnRegistered(UnitOfWorkInterceptorRegistrar.RegisterIfNeeded);
        return base.OnConfigureAsync(context);
    }
}
