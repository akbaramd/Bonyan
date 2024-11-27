using System.Collections.Generic;
using System.Threading.Tasks;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bonyan.Messaging.RabbitMQ.Tests;

public class RabbitMqDispatcherTests
{
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqDispatcherTests()
    {
        var services = new ServiceCollection();

        // Initialize modular application with RabbitMQModule
        var application = new BonModularityApplication<RabbitMQModule>(services, c =>
        {
            c.ApplicationName = nameof(RabbitMqDispatcherTests);
        });
        application.ConfigureModulesAsync().GetAwaiter().GetResult();
        application.InitializeModulesAsync(application.ServiceProvider).GetAwaiter().GetResult();

        _serviceProvider = application.ServiceProvider;
    }

    [Fact]
    public async Task PublishAsync_ShouldDeliverMessageToAllSubscribers()
    {
        // Arrange
        var bus = _serviceProvider.GetRequiredService<IBonMessageBus>();
        var message = new TestEvent { Content = "Test Publish Event" };

        var subscriber1Handled = false;
        var subscriber2Handled = false;

        // Subscribe two handlers
        bus.Subscribe<TestEvent>("subscriber1.queue", async (context) =>
        {
            if (context.Message.Content == message.Content)
                subscriber1Handled = true;

            await Task.CompletedTask;
        });

        bus.Subscribe<TestEvent>("subscriber2.queue", async (context) =>
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
        var message = new TestCommand { Content = "Targeted Send Command" };

        var targetedHandlerCalled = false;

        // Subscribe to a specific queue
        bus.Subscribe<TestCommand>("targeted.queue",  async (context) =>
        {
            if (context.Message.Content == message.Content)
                targetedHandlerCalled = true;

            await Task.CompletedTask;
        });

        // Act
        await bus.SendAsync("targeted.queue", message);

        // Allow time for message processing
        await Task.Delay(500);

        // Assert
        Assert.True(targetedHandlerCalled, "Targeted handler did not process the message.");
    }

    [Fact]
    public async Task SendAsync_ShouldThrowTimeoutExceptionForNoResponse()
    {
        // Arrange
        var bus = _serviceProvider.GetRequiredService<IBonMessageBus>();
        var command = new TestCommand { Content = "No Response Command" };

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() =>
            bus.SendAsync<TestCommand, TestResponse>("nonexistent.queue", command));
    }

    [Fact]
    public async Task SendAsync_ShouldReceiveExpectedResponse()
    {
        // Arrange
        var bus = _serviceProvider.GetRequiredService<IBonMessageBus>();
        var command = new TestCommand { Content = "Request" };

        // Set up a consumer to respond
        bus.Subscribe<TestCommand>("response.queue", async (context) =>
        {
            if (context.Message.Content == "Request")
            {
                var response = new TestResponse { Content = "Response to Request" };
                await context.ReplyAsync(response);
            }
        });

        // Act
        var response = await bus.SendAsync<TestCommand, TestResponse>("response.queue", command);

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

    [Fact]
    public async Task Subscribe_ShouldProcessMultipleMessages()
    {
        // Arrange
        var bus = _serviceProvider.GetRequiredService<IBonMessageBus>();
        var messagesHandled = new List<string>();

        bus.Subscribe<TestEvent>("multi-message.queue", async (context) =>
        {
            messagesHandled.Add(context.Message.Content);
            await Task.CompletedTask;
        });

        // Act
        await bus.PublishAsync(new TestEvent { Content = "Message 1" });
        await bus.PublishAsync(new TestEvent { Content = "Message 2" });
        await bus.PublishAsync(new TestEvent { Content = "Message 3" });

        // Allow time for message processing
        await Task.Delay(500);

        // Assert
        Assert.Contains("Message 1", messagesHandled);
        Assert.Contains("Message 2", messagesHandled);
        Assert.Contains("Message 3", messagesHandled);
    }

    [Fact]
    public async Task SendAsync_ShouldHandleReplyQueueBindingCorrectly()
    {
        // Arrange
        var bus = _serviceProvider.GetRequiredService<IBonMessageBus>();
        var command = new TestCommand { Content = "Reply Binding Test" };

        // Set up a consumer to respond
        bus.Subscribe<TestCommand>("reply.binding.queue", async (context) =>
        {
            if (context.Message.Content == "Reply Binding Test")
            {
                var response = new TestResponse { Content = "Reply Test Successful" };
                await context.ReplyAsync(response);
            }
        });

        // Act
        var response = await bus.SendAsync<TestCommand, TestResponse>("reply.binding.queue", command);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Reply Test Successful", response.Content);
    }

    [Fact]
    public async Task Subscribe_ShouldHandleNoMessagesGracefully()
    {
        // Arrange
        var bus = _serviceProvider.GetRequiredService<IBonMessageBus>();
        var messagesHandled = false;

        bus.Subscribe<TestEvent>("empty.queue", async (context) =>
        {
            messagesHandled = true;
            await Task.CompletedTask;
        });

        // Act
        // No messages are published to "empty.queue"

        // Assert
        await Task.Delay(500); // Give time to verify no messages are handled
        Assert.False(messagesHandled, "Handler incorrectly processed a non-existent message.");
    }
}
