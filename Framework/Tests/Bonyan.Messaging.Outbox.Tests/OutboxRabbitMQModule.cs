using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.OutBox;
using Bonyan.Messaging.RabbitMQ;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Messaging.Outbox.Tests;

/// <summary>
/// Test module for outbox pattern with RabbitMQ integration.
/// Automatically loads all required dependencies and configures services.
/// </summary>
public class OutboxRabbitMQModule : BonModule
{
    public OutboxRabbitMQModule()
    {
        // Automatically load all required modules
        DependOn<BonMessagingRabbitMqModule>();
        DependOn<BonMessagingOutboxModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
    


        return Task.CompletedTask;
    }

    public override Task OnInitializeAsync(BonInitializedContext context)
    {
        // Initialize any test-specific components
        return Task.CompletedTask;
    }
}