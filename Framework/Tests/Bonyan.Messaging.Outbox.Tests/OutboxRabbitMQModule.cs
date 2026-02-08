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

    public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
    {
    


        return ValueTask.CompletedTask;
    }

    public override ValueTask OnInitializeAsync(BonInitializedContext context , CancellationToken cancellationToken = default)
    {
        // Initialize any test-specific components
        return ValueTask.CompletedTask;
    }
}