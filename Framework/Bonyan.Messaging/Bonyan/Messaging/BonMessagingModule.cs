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
            var preActions = context.Services.GetPreConfigureActions<BonMessagingOptions>();
            
            context.Services.AddBonMessaging(c => { preActions.Configure(c); });

            return base.OnPostConfigureAsync(context);
        }
    }
}