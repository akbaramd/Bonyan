using Bonyan.Messaging.Abstractions;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Workers;
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
             
                c.AddRabbitMQ(r => { context.Services.ExecutePreConfiguredActions(r); });
            });
            return base.OnConfigureAsync(context);
        }

        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {
          
            return base.OnPostConfigureAsync(context);
        }
    }
}