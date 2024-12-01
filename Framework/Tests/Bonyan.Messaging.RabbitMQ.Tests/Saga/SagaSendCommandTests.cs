using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.Saga;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging.RabbitMQ.Tests.Saga
{
    public class SagaSendCommandTests
    {
        private readonly IServiceProvider _serviceProvider;

        public SagaSendCommandTests()
        {
            // Initialize modular application with the module containing the saga
            var builder = BonyanApplication.CreateModularBuilder<RabbitMqModule>(nameof(SagaSendCommandTests));
            var app = builder.BuildAsync().GetAwaiter().GetResult();
            app.StartAsync();
            _serviceProvider = app.Services;
        }

        [Fact]
        public async Task Saga_ShouldSendCompleteCommandToQueue()
        {
            // Arrange
            var stateStore = _serviceProvider.GetRequiredService<IBonStateStore>();
            var bus = _serviceProvider.GetRequiredService<IBonMessageBus>();
            var subscriber = _serviceProvider.GetRequiredService<IBonMessageSubscriber>();

            var inProgressEvent = new StartEvent() { Content = "12345" };
            var completeCommandReceived = false;

            // Subscribe to the complete-command.queue to verify message
            subscriber.Subscribe<CompleteCommand>("complete-command.queue", async context =>
            {
                if (context.Message.Content == "Complete Processing")
                {
                    completeCommandReceived = true;
                }
                await Task.CompletedTask;
            });

            // Act
            await bus.PublishAsync(inProgressEvent, correlationId: "order-12345");
            await Task.Delay(500); // Allow time for message processing

            // Assert
            Assert.True(completeCommandReceived, "The CompleteCommand was not received on the queue.");
            
     
        }
    }


}
