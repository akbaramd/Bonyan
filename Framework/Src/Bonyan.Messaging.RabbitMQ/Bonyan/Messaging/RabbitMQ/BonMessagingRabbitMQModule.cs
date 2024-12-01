using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging.RabbitMQ
{
    public class BonMessagingRabbitMqModule : BonModule
    {
        public BonMessagingRabbitMqModule()
        {
            DependOn<BonMessagingModule>();
        }

      public override Task OnPreConfigureAsync(BonConfigurationContext context)
      {
          PreConfigure<BonMessagingConfiguration>(c =>
          {
              c.AddRabbitMq(r => { context.Services.ExecutePreConfiguredActions(r); });
          });
          return base.OnPreConfigureAsync(context);
      }

      public override Task OnConfigureAsync(BonConfigurationContext context)
        {
           
            return base.OnConfigureAsync(context);
        }

        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {
            
            return base.OnPostConfigureAsync(context);
        }
    }
}