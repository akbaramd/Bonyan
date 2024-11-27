using Bonyan.Mediators;
using Bonyan.Messaging;
using Bonyan.Modularity;

namespace Microsoft.Extensions.DependencyInjection;

public static class BonMediatorServiceCollectionExtensions
{
    public static BonMediatorConfiguration AddEventMessaging(
        this BonMediatorConfiguration context)
    {
        context.Context.Services.AddTransient (typeof(IBonMediatorBehavior<>),typeof(BonMediatorMessagingBehavior<>));
        return context;
    }

 
}