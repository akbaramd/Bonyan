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

      public override ValueTask OnPreConfigureAsync(BonPreConfigurationContext context , CancellationToken cancellationToken = default)
      {
          context.Services.PreConfigure<BonMessagingConfiguration>(c =>
          {
              c.AddRabbitMq(r => { context.Services.ExecutePreConfiguredActions(r); });
          });
          return base.OnPreConfigureAsync(context);
      }

      public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
        {
           
            return base.OnConfigureAsync(context);
        }

        public override ValueTask OnPostConfigureAsync(BonPostConfigurationContext context , CancellationToken cancellationToken = default)
        {
            
            return base.OnPostConfigureAsync(context);
        }
    }
}