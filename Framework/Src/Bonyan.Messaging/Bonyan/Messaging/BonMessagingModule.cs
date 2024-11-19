using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging
{
    public class BonMessagingModule : BonModule
    {
        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {
            var service = context.Services.GetObject<BonServiceOptions>();

            context.AddMessaging(service.ServiceName, c =>
            {
                context.Services.ExecutePreConfiguredActions(c);
            });
        

            return base.OnPostConfigureAsync(context);
        }
    }
}