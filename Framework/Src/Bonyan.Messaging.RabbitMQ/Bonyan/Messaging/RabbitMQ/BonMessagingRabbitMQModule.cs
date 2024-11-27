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
            DependOn<BonWorkersModule>();
        }

        public override Task OnPreConfigureAsync(BonConfigurationContext context)
        {
            PreConfigure<BonWorkerConfiguration>(c =>
            {
                c.RegisterWorker<BonConsumersWorkers>();
            });
            return base.OnPreConfigureAsync(context);
        }

        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
            context.RegisterScopedServicesFor(typeof(IBonMessageConsumer<>));
            
            PreConfigure<BonMessagingConfiguration>(c =>
            {
             
                c.AddRabbitMqMessaging(r => { context.Services.ExecutePreConfiguredActions(r); });
            });
            return base.OnConfigureAsync(context);
        }

        public override Task OnPostConfigureAsync(BonConfigurationContext context)
        {
          
            return base.OnPostConfigureAsync(context);
        }
    }
}