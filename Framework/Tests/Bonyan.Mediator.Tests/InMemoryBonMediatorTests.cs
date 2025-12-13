using Bonyan.Mediators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bonyan.Mediator.Tests;

public class InMemoryBonMediatorTests
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryBonMediatorTests()
    {
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole());

        // Register mediator
        services.AddTransient<IBonMediator, InMemoryBonMediator>();

        // Register handlers
        services.AddTransient<IBonCommandHandler<SampleCommand, string>, SampleCommandHandler>();
        services.AddTransient<IBonCommandHandler<SampleQuery, string>, SampleQueryHandler>();
        services.AddTransient<IBonEventHandler<SampleEvent>, SampleEventHandler>();

        // Register behaviors
        services.AddTransient(typeof(IBonMediatorBehavior<>), typeof(SampleEventMediatorBehavior<>));
        services.AddTransient(typeof(IBonMediatorBehavior<,>), typeof(SampleCommandMediatorBehavior<,>));

        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task SendAsync_ShouldHandleCommandWithResponse()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IBonMediator>();
        var command = new SampleCommand { Payload = TestStrings.TestCommandPayload };

        // Act
        var result = await mediator.SendAsync(command);

        // Assert
        Assert.Equal(TestStrings.TestCommandHandled, result);
    }

    [Fact]
    public async Task SendAsync_ShouldHandleCommandWithBehavior()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IBonMediator>();
        var command = new SampleCommand { Payload = TestStrings.TestCommandPayload };

        // Act
        var result = await mediator.SendAsync(command);

        // Assert
        Assert.Equal(TestStrings.TestCommandHandled, result);

        // Additional validation can capture console output if needed.
        // Example:
        // AssertConsoleOutputContains(TestStrings.BehaviorPreProcessingMessage + TestStrings.TestCommandPayload);
        // AssertConsoleOutputContains(TestStrings.BehaviorPostProcessingMessage + TestStrings.TestCommandPayload);
    }

    [Fact]
    public async Task SendAsync_ShouldHandleQueryAsCommand()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IBonMediator>();
        var query = new SampleQuery { Query = TestStrings.TestQueryPayload };

        // Act
        var result = await mediator.SendAsync(query);

        // Assert
        Assert.Equal(TestStrings.TestQueryResult, result);
    }

    [Fact]
    public async Task PublishAsync_ShouldHandleEvent()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IBonMediator>();
        var @event = new SampleEvent { EventData = TestStrings.TestEventPayload };

        // Act
        await mediator.PublishAsync(@event);

        // Assert
        // Validate console output or additional state changes if necessary
    }

    [Fact]
    public async Task PublishAsync_ShouldHandleEventWithBehavior()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IBonMediator>();
        var @event = new SampleEvent { EventData = TestStrings.TestEventPayload };

        // Act
        await mediator.PublishAsync(@event);

        // Assert
        // Additional validation can capture console output if needed.
        // Example:
        // AssertConsoleOutputContains(TestStrings.BehaviorPreProcessingMessage + TestStrings.TestEventPayload);
        // AssertConsoleOutputContains(TestStrings.BehaviorPostProcessingMessage + TestStrings.TestEventPayload);
    }

    [Fact]
    public async Task SendAsync_ShouldThrowExceptionIfNoHandlerFound()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IBonMediator>();
        var command = new SampleCommand { Payload = "UnregisteredCommand" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.SendAsync(new UnregisteredCommand()));
    }

    [Fact]
    public async Task SendAsync_ShouldThrowExceptionIfNoHandlerFoundForQuery()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IBonMediator>();
        var query = new UnregisteredQuery { Query = "UnregisteredQuery" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.SendAsync(query));
    }

    [Fact]
    public async Task PublishAsync_ShouldNotThrowIfNoEventHandlersFound()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IBonMediator>();
        var @event = new UnregisteredEvent { EventData = "UnregisteredEvent" };

        // Act
        var exception = await Record.ExceptionAsync(() => mediator.PublishAsync(@event));

        // Assert
        Assert.Null(exception); // Should gracefully handle missing event handlers
    }
}

// Define test entities for unregistered handlers
public class UnregisteredCommand : IBonCommand<string>
{
    public string Payload { get; set; }
}

public class UnregisteredQuery : IBonCommand<string>
{
    public string Query { get; set; }
}

public class UnregisteredEvent : IBonEvent
{
    public string EventData { get; set; }
}