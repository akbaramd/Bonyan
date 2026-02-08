using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.OutBox;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Messaging.Outbox.Tests;

/// <summary>
/// Simple test module for outbox pattern testing.
/// Automatically loads all required dependencies.
/// </summary>
public class OutboxTestModule : BonModule
{
    public OutboxTestModule()
    {
        // Automatically load all required modules
        DependOn<BonMessagingOutboxModule>();
    }

}
