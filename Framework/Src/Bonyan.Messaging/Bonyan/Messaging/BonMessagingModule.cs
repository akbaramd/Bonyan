using Bonyan.Messaging.Abstractions;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging
{
    public class BonMessagingModule : BonModule
    {
        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
            return base.OnConfigureAsync(context);
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