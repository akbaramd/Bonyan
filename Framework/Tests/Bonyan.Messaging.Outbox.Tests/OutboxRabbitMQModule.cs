using Bonyan.Messaging.OutBox;
using Bonyan.Messaging.RabbitMQ;
using Bonyan.Modularity;

namespace Bonyan.Messaging.Outbox.Tests;

public class OutboxRabbitMQModule : BonWebModule
{

    public OutboxRabbitMQModule()
    {
        DependOn<BonMessagingRabbitMqModule>();
        DependOn<BonMessagingOutboxModule>();
    }
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        
        PreConfigure<BonRabbitMqConfiguration>(options =>
        {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
            options.VirtualHost = "/";
     
        });

        return Task.CompletedTask;
    }
}