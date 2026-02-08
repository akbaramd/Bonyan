using Bonyan.Mediators;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging
{
    public class BonMessagingModule : BonModule
    {
        public BonMessagingModule()
        {
            DependOn<BonMediatorModule>();
        }

        public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
        {
            context.AddMessaging( c =>
            {
                context.Services.ExecutePreConfiguredActions(c);
            });
            
            return base.OnConfigureAsync(context);
        }
    }
}