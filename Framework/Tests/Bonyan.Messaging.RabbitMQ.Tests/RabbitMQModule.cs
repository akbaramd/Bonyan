using Bonyan.Messaging.RabbitMQ.Tests.Saga;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Messaging.RabbitMQ.Tests;

public class RabbitMqModule : BonModule
{

    public RabbitMqModule()
    {
        DependOn<BonMessagingRabbitMqModule>();
    }
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        PreConfigure<BonMessagingConfiguration>(c =>
        {
            c.RegisterConsumer<TestEventConsumer>();
            c.RegisterSaga<TestMessagingSaga,TestSagaInstance>();
        });
        
        PreConfigure<BonRabbitMqConfiguration>(options =>
        {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
            options.VirtualHost = "/";
            
            
            options.ConfigureConsumer<TestEventConsumer>("test-event");
            options.ConfigureSaga<TestMessagingSaga,TestSagaInstance>("test-event");
        });

        return Task.CompletedTask;
    }
}