using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.UnitOfWork;

public class BonUnitOfWorkModule : BonModule
{
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddTransient<BonUnitOfWorkInterceptor>();
        context.Services.AddTransient<IBonUnitOfWork, BonUnitOfWork>();
        context.Services.AddSingleton<IBonUnitOfWorkManager, BonUnitOfWorkManager>();
        
        context.Services.AddSingleton<IUnitOfWorkTransactionBehaviourProvider, NullUnitOfWorkTransactionBehaviourProvider>();
        context.Services.AddSingleton<IUnitOfWorkEventPublisher, NullUnitOfWorkEventPublisher>();
        context.Services.AddSingleton<IBonAmbientBonUnitOfWork, BonAmbientBonUnitOfWork>();
        context.Services.AddSingleton<IBonUnitOfWorkAccessor, BonAmbientBonUnitOfWork>();
        
        context.Services.OnRegistered(UnitOfWorkInterceptorRegistrar.RegisterIfNeeded);
        return base.OnConfigureAsync(context);
    }
}
