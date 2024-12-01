using Bonyan.Mediators;
using Bonyan.Mediators.Messaging;

namespace Microsoft.Extensions.DependencyInjection;

public static class BonMediatorServiceCollectionExtensions
{
    public static BonMediatorConfiguration UseMessagingForDomainEvent(
        this BonMediatorConfiguration context)
    {
        context.Context.Services.AddTransient (typeof(IBonMediatorBehavior<>),typeof(BonMediatorMessagingBehavior<>));
        return context;
    }

 
}