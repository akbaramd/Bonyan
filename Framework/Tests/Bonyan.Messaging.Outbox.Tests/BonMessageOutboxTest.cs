using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.OutBox;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging.Outbox.Tests;

public class BonMessageOutboxTest
{
    private readonly IServiceProvider _serviceProvider;

    public BonMessageOutboxTest()
    {
        var services = new ServiceCollection();

        // Initialize modular application with RabbitMQModule
        var builder = BonyanApplication.CreateModularBuilder<OutboxRabbitMQModule>(nameof(BonMessageOutboxTest));

        var app = builder.BuildAsync().GetAwaiter().GetResult();
        app.StartAsync().GetAwaiter().GetResult();
        _serviceProvider = app.Services;
    }

    [Fact]
    public async Task PublishAsync_ShouldDeliverMessageToAllSubscribers()
    {
        // Arrange
        var bus = _serviceProvider.GetRequiredService<IBonMessageBus>();
        var sore = _serviceProvider.GetRequiredService<IOutboxStore>();
        var subscriber = _serviceProvider.GetRequiredService<IBonMessageSubscriber>();
        var message = new TestEvent { Content = "Test Publish Event" };

        var subscriber1Queue = $"subscriber1.queue.{Guid.NewGuid()}";
        var subscriber2Queue = $"subscriber2.queue.{Guid.NewGuid()}";

        var subscriber1Handled = new TaskCompletionSource<bool>();
        var subscriber2Handled = new TaskCompletionSource<bool>();

        // Subscribe two handlers with unique queues
        subscriber.Subscribe<TestEvent>(subscriber1Queue, async (context) =>
        {
            if (context.Message.Content == message.Content)
                subscriber1Handled.SetResult(true);

            await Task.CompletedTask;
        });

        subscriber.Subscribe<TestEvent>(subscriber2Queue, async (context) =>
        {
            if (context.Message.Content == message.Content)
                subscriber2Handled.SetResult(true);

            await Task.CompletedTask;
        });

        // Act
        await bus.PublishAsync(message);

        // Assert
        var subscriber1Task = subscriber1Handled.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var subscriber2Task = subscriber2Handled.Task.WaitAsync(TimeSpan.FromSeconds(5));

        await Task.WhenAll(subscriber1Task, subscriber2Task);

        Assert.True(await subscriber1Task, "Subscriber 1 did not handle the published message.");
        Assert.True(await subscriber2Task, "Subscriber 2 did not handle the published message.");
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
        Assert.Equal("Response to Request", response.Content);
    }


   
}