using Bonyan.Messaging.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging.RabbitMQ.Tests;

public class BonMessageBusTest
{
    private readonly IServiceProvider _serviceProvider;

    public BonMessageBusTest()
    {
        var services = new ServiceCollection();

        // Initialize modular application with RabbitMQModule
        var builder = BonyanApplication.CreateModularBuilder<RabbitMqModule>(nameof(BonMessageBusTest));

        var app = builder.BuildAsync().GetAwaiter().GetResult();
        app.StartAsync();
        _serviceProvider = app.Services;
    }

    [Fact]
    public async Task PublishAsync_ShouldDeliverMessageToAllSubscribers()
    {
        // Arrange
        var bus = _serviceProvider.GetRequiredService<IBonMessageBus>();
        var subscriber = _serviceProvider.GetRequiredService<IBonMessageSubscriber>();
        var message = new TestEvent { Content = "Test Publish Event" };

        var subscriber1Queue = $"subscriber1.queue.{Guid.NewGuid()}";
        var subscriber2Queue = $"subscriber2.queue.{Guid.NewGuid()}";

        var subscriber1Handled = false;
        var subscriber2Handled = false;

        // Subscribe two handlers with unique queues
        subscriber.Subscribe<TestEvent>(subscriber1Queue, async (context) =>
        {
            if (context.Message.Content == message.Content)
                subscriber1Handled = true;

            await Task.CompletedTask;
        });

        subscriber.Subscribe<TestEvent>(subscriber2Queue, async (context) =>
        {
            if (context.Message.Content == message.Content)
                subscriber2Handled = true;

            await Task.CompletedTask;
        });

        // Act
        await bus.PublishAsync(message);

        // Allow time for message processing
        await Task.Delay(500);

        // Assert
        Assert.True(subscriber1Handled, "Subscriber 1 did not handle the published message.");
        Assert.True(subscriber2Handled, "Subscriber 2 did not handle the published message.");
    }

    [Fact]
    public async Task SendAsync_ShouldDeliverMessageToSpecificSubscriber()
    {
        // Arrange
        var bus = _serviceProvider.GetRequiredService<IBonMessageBus>();
        var subscriber = _serviceProvider.GetRequiredService<IBonMessageSubscriber>();
        var queueName = $"targeted.queue.{Guid.NewGuid()}";
        var message = new TestCommand { Content = "Targeted Send Command" };

        var targetedHandlerCalled = false;

        // Subscribe to a specific unique queue
        subscriber.Subscribe<TestCommand>(queueName, async (context) =>
        {
            if (context.Message.Content == message.Content)
                targetedHandlerCalled = true;

            await Task.CompletedTask;
        });

        // Act
        await bus.SendAsync(queueName, message);

        // Allow time for message processing
        await Task.Delay(500);

        // Assert
        Assert.True(targetedHandlerCalled, "Targeted handler did not process the message.");
    }

    [Fact]
    public async Task SendAsync_ShouldReceiveExpectedResponse()
    {
        // Arrange
        var bus = _serviceProvider.GetRequiredService<IBonMessageBus>();
        var subscriber = _serviceProvider.GetRequiredService<IBonMessageSubscriber>();
        var queueName = $"response.queue.{Guid.NewGuid()}";
        var command = new TestCommand { Content = "Request" };

        // Subscribe to provide a response
        subscriber.Subscribe<TestCommand>(queueName, async (context) =>
        {
            if (context.Message.Content == "Request")
            {
                var response = new TestResponse { Content = "Response to Request" };
                await context.ReplyAsync(response);
            }
        });

        // Act
        var response = await bus.SendAsync<TestCommand, TestResponse>(queueName, command);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Response to Request", response.Content);
    }

    [Fact]
    public async Task PublishAsync_ShouldNotFailForNoSubscribers()
    {
        // Arrange
        var bus = _serviceProvider.GetRequiredService<IBonMessageBus>();
        var message = new TestEvent { Content = "Unsubscribed Event" };

        // Act
        var exception = await Record.ExceptionAsync(() => bus.PublishAsync(message));

        // Assert
        Assert.Null(exception); // Should gracefully handle no subscribers
    }
}