using Bonyan.Messaging.Abstractions;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging
{
    public class BonMessagingModule : BonModule
    {
        public override Task OnPreConfigureAsync(BonConfigurationContext context)
        {
            context.Services.AddHostedService<BonBackgroundConsumerService>();
            context.Services.AddHostedService<BonSagaBackgroundService>();
            return base.OnPreConfigureAsync(context);
        }


        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {
            context.AddMessaging( c =>
            {
                context.Services.ExecutePreConfiguredActions(c);
            });
            
            return base.OnPostConfigureAsync(context);
        }
    }
}