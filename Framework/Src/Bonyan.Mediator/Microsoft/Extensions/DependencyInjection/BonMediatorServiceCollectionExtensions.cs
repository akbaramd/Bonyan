using Bonyan.Messaging;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.Abstractions.Mediators;
using Bonyan.Messaging.Mediators;
using Bonyan.Modularity;

namespace Microsoft.Extensions.DependencyInjection;

public static class BonMediatorServiceCollectionExtensions
{
    public static BonConfigurationContext AddMediator(
        this BonConfigurationContext context)
    {
        context.Services.AddTransient<IBonMediator, InMemoryBonMediator>();
        context.RegisterTransientServicesFor(typeof(IBonCommandHandler<>));
        context.RegisterTransientServicesFor(typeof(IBonCommandHandler<,>));
        context.RegisterTransientServicesFor(typeof(IBonQueryHandler<,>));
        context.RegisterTransientServicesFor(typeof(IBonEventHandler<>));
        return context;
    }
}