using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging.RabbitMQ
{
    public class BonMessagingRabbitMQModule : BonModule
    {
        public BonMessagingRabbitMQModule()
        {
            DependOn<BonMessagingModule>();
        }

        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
            PreConfigure<BonMessagingConfiguration>(c =>
            {
                var pres = context.Services.GetPreConfigureActions<RabbitMQOptions>();
                c.AddRabbitMqMessaging(r => { pres.Configure(r); });
            });
            return base.OnConfigureAsync(context);
        }

        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {
          
            return base.OnPostConfigureAsync(context);
        }
    }
}