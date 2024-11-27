using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Messaging.RabbitMQ.Tests;

public class RabbitMQModule : BonModule
{

    public RabbitMQModule()
    {
        DependOn<BonMessagingRabbitMQModule>();
    }
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        PreConfigure<RabbitMQOptions>(options =>
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