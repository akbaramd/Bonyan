using Bonyan.Mediators;
using Bonyan.Modularity;

namespace Microsoft.Extensions.DependencyInjection;

public static class BonMediatorServiceCollectionExtensions
{
    public static BonConfigurationContext AddMediator(
        this BonConfigurationContext context  , Action<BonMediatorConfiguration>? configureOptions = null)
    {
        // Register core mediator services
        context.Services.AddTransient<IBonMediator, InMemoryBonMediator>();

        // Register handlers
        context.RegisterTransientServicesFor(typeof(IBonCommandHandler<>));
        context.RegisterTransientServicesFor(typeof(IBonCommandHandler<,>));
        context.RegisterTransientServicesFor(typeof(IBonQueryHandler<,>));
        context.RegisterTransientServicesFor(typeof(IBonEventHandler<>));
        
        
        var options = new BonMediatorConfiguration(context);

        configureOptions?.Invoke(options);

        return context;
    }

 
}