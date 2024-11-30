using Bonyan.Messaging;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Mediators.Messaging
{
    public class BonMediatorMessagingModule : BonModule
    {
        public BonMediatorMessagingModule()
        {
            DependOn<BonMediatorModule>();
            DependOn<BonMessagingModule>();
        }

        public override Task OnPreConfigureAsync(BonConfigurationContext context)
        {
            PreConfigure<BonMediatorConfiguration>(c => { c.AddEventMessaging(); });
            return base.OnPreConfigureAsync(context);
        }
    }
}