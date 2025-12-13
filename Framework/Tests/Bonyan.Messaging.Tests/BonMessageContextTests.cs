using Bonyan.Mediators;
using Bonyan.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bonyan.Messaging.Tests;

public class BonMessageContextTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IBonMessageBus _messageBus;

    public BonMessageContextTests()
    {
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole());

        // Register mediator
        services.AddTransient<IBonMediator, InMemoryBonMediator>();

        // Register handlers
        services.AddTransient<IBonCommandHandler<TestMessageRequestWithResponse, TestResponse>, TestMessageRequestWithResponseHandler>();
        services.AddTransient<IBonCommandHandler<TestMessageRequest>, TestMessageRequestHandler>();
        services.AddTransient<IBonEventHandler<TestMessageEvent>, TestMessageEventHandler>();

        // Register message bus
        services.AddSingleton<IBonMessageBus>(sp =>
        {
            var mediator = sp.GetRequiredService<IBonMediator>();
            var logger = sp.GetRequiredService<ILogger<MediatorBonMessageBus>>();
            return new MediatorBonMessageBus(mediator, sp, logger);
        });

        _serviceProvider = services.BuildServiceProvider();
        _messageBus = _serviceProvider.GetRequiredService<IBonMessageBus>();
    }

    [Fact]
    public void BonMessageContext_ShouldInitializeCorrectly()
    {
        // Arrange
        var message = new TestMessage { Data = "Test data" };
        var correlationId = "test-correlation-id";
        var headers = new Dictionary<string, object> { { "Key", "Value" } };
        var replyTo = "reply-service";

        // Act
        var context = new BonMessageContext<TestMessage>(
            message, correlationId, headers, replyTo, _serviceProvider);

        // Assert
        Assert.Equal(message, context.Message);
        Assert.Equal(correlationId, context.CorrelationId);
        Assert.Equal(headers, context.Headers);
        Assert.Equal(replyTo, context.ReplyTo);
    }

    [Fact]
    public void BonMessageContext_WithNullHeaders_ShouldCreateEmptyDictionary()
    {
        // Arrange
        var message = new TestMessage { Data = "Test data" };
        var correlationId = "test-correlation-id";
        var replyTo = "reply-service";

        // Act
        var context = new BonMessageContext<TestMessage>(
            message, correlationId, null, replyTo, _serviceProvider);

        // Assert
        Assert.NotNull(context.Headers);
        Assert.Empty(context.Headers);
    }

    [Fact]
    public void BonMessageContext_ShouldThrowOnNullArguments()
    {
        // Arrange
        var message = new TestMessage { Data = "Test data" };
        var correlationId = "test-correlation-id";
        var replyTo = "reply-service";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new BonMessageContext<TestMessage>(null!, correlationId, null, replyTo, _serviceProvider));

        Assert.Throws<ArgumentNullException>(() =>
            new BonMessageContext<TestMessage>(message, null!, null, replyTo, _serviceProvider));

        Assert.Throws<ArgumentNullException>(() =>
            new BonMessageContext<TestMessage>(message, correlationId, null, null!, _serviceProvider));
    }

    [Fact]
    public async Task ReplyAsync_ShouldSendResponseCorrectly()
    {
        // Arrange
        var message = new TestMessage { Data = "Test data" };
        var correlationId = "test-correlation-id";
        var replyTo = "reply-service";
        var context = new BonMessageContext<TestMessage>(
            message, correlationId, null, replyTo, _serviceProvider);

        var response = new TestResponse { Result = "Test response" };

        // Act & Assert
        // This should not throw an exception
        await context.ReplyAsync(response);
        Assert.True(true);
    }

    [Fact]
    public async Task ReplyAsync_WithHeaders_ShouldIncludeHeaders()
    {
        // Arrange
        var message = new TestMessage { Data = "Test data" };
        var correlationId = "test-correlation-id";
        var replyTo = "reply-service";
        var context = new BonMessageContext<TestMessage>(
            message, correlationId, null, replyTo, _serviceProvider);

        var response = new TestResponse { Result = "Test response" };
        var headers = new Dictionary<string, object> { { "Priority", "High" } };

        // Act & Assert
        await context.ReplyAsync(response, headers);
        Assert.True(true);
    }

    [Fact]
    public async Task PublishAsync_ShouldPublishEventCorrectly()
    {
        // Arrange
        var message = new TestMessage { Data = "Test data" };
        var correlationId = "test-correlation-id";
        var replyTo = "reply-service";
        var context = new BonMessageContext<TestMessage>(
            message, correlationId, null, replyTo, _serviceProvider);

        var @event = new TestMessageEvent { EventData = "Test event" };

        // Act & Assert
        await context.PublishAsync(@event);
        Assert.Equal(correlationId, @event.CorrelationId);
    }

    [Fact]
    public async Task PublishAsync_WithHeaders_ShouldSetHeaders()
    {
        // Arrange
        var message = new TestMessage { Data = "Test data" };
        var correlationId = "test-correlation-id";
        var replyTo = "reply-service";
        var context = new BonMessageContext<TestMessage>(
            message, correlationId, null, replyTo, _serviceProvider);

        var @event = new TestMessageEvent { EventData = "Test event" };
        var headers = new Dictionary<string, object> { { "EventType", "Test" } };

        // Act
        await context.PublishAsync(@event, headers);

        // Assert
        Assert.Equal(correlationId, @event.CorrelationId);
        Assert.Equal(headers, @event.Headers);
    }

    [Fact]
    public async Task SendAsync_WithoutResponse_ShouldSendRequestCorrectly()
    {
        // Arrange
        var message = new TestMessage { Data = "Test data" };
        var correlationId = "test-correlation-id";
        var replyTo = "reply-service";
        var context = new BonMessageContext<TestMessage>(
            message, correlationId, null, replyTo, _serviceProvider);

        var request = new TestMessageRequest { Data = "Test request" };

        // Act & Assert
        await context.SendAsync("target-service", request);
        Assert.Equal(correlationId, request.CorrelationId);
        Assert.Equal("target-service", request.TargetService);
    }

    [Fact]
    public async Task SendAsync_WithResponse_ShouldSendRequestAndReturnResponse()
    {
        // Arrange
        var message = new TestMessage { Data = "Test data" };
        var correlationId = "test-correlation-id";
        var replyTo = "reply-service";
        var context = new BonMessageContext<TestMessage>(
            message, correlationId, null, replyTo, _serviceProvider);

        var request = new TestMessageRequestWithResponse { Data = "Test request" };

        // Act
        var response = await context.SendAsync<TestMessageRequestWithResponse, TestResponse>(
            "target-service", request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(correlationId, request.CorrelationId);
        Assert.Equal("target-service", request.TargetService);
    }

    [Fact]
    public async Task SendAsync_WithHeaders_ShouldSetHeaders()
    {
        // Arrange
        var message = new TestMessage { Data = "Test data" };
        var correlationId = "test-correlation-id";
        var replyTo = "reply-service";
        var context = new BonMessageContext<TestMessage>(
            message, correlationId, null, replyTo, _serviceProvider);

        var request = new TestMessageRequest { Data = "Test request" };
        var headers = new Dictionary<string, object> { { "Priority", "High" } };

        // Act
        await context.SendAsync("target-service", request, headers);

        // Assert
        Assert.Equal(correlationId, request.CorrelationId);
        Assert.Equal("target-service", request.TargetService);
        Assert.Equal(headers, request.Headers);
    }

    [Fact]
    public void SendAsync_ShouldThrowOnNullMessage()
    {
        // Arrange
        var message = new TestMessage { Data = "Test data" };
        var correlationId = "test-correlation-id";
        var replyTo = "reply-service";
        var context = new BonMessageContext<TestMessage>(
            message, correlationId, null, replyTo, _serviceProvider);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            context.SendAsync("target-service", (TestMessageRequest)null!));

        Assert.ThrowsAsync<ArgumentNullException>(() =>
            context.SendAsync<TestMessageRequestWithResponse, TestResponse>(
                "target-service", (TestMessageRequestWithResponse)null!));
    }

    [Fact]
    public void PublishAsync_ShouldThrowOnNullEvent()
    {
        // Arrange
        var message = new TestMessage { Data = "Test data" };
        var correlationId = "test-correlation-id";
        var replyTo = "reply-service";
        var context = new BonMessageContext<TestMessage>(
            message, correlationId, null, replyTo, _serviceProvider);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            context.PublishAsync((TestMessageEvent)null!));
    }

    [Fact]
    public void ReplyAsync_ShouldThrowOnNullResponse()
    {
        // Arrange
        var message = new TestMessage { Data = "Test data" };
        var correlationId = "test-correlation-id";
        var replyTo = "reply-service";
        var context = new BonMessageContext<TestMessage>(
            message, correlationId, null, replyTo, _serviceProvider);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            context.ReplyAsync((TestResponse)null!));
    }
}

/// <summary>
/// Test message for context testing
/// </summary>
public class TestMessage
{
    public string Data { get; set; } = string.Empty;
}

public class TestResponse
{
    public string Result { get; set; } = string.Empty;
}

public class TestMessageEvent : MessageEventBase
{
    public string EventData { get; set; } = string.Empty;
}

public class TestMessageRequest : MessageRequestBase
{
    public string Data { get; set; } = string.Empty;
}

public class TestMessageRequestWithResponse : MessageRequestBase<TestResponse>
{
    public string Data { get; set; } = string.Empty;
}

public class TestMessageRequestWithResponseHandler : IBonCommandHandler<TestMessageRequestWithResponse, TestResponse>
{
    public Task<TestResponse> HandleAsync(TestMessageRequestWithResponse request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new TestResponse { Result = $"Processed: {request.Data}" });
    }
}

public class TestMessageRequestHandler : IBonCommandHandler<TestMessageRequest>
{
    public Task HandleAsync(TestMessageRequest request, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

public class TestMessageEventHandler : IBonEventHandler<TestMessageEvent>
{
    public Task HandleAsync(TestMessageEvent @event, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
