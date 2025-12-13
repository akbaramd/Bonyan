using Bonyan.Mediators;
using Bonyan.Messaging.Abstractions;

namespace Bonyan.Messaging.Tests;

public class MessageInterfaceTests
{
    [Fact]
    public void MessageRequestBase_ShouldInitializeCorrectly()
    {
        // Act
        var request = new TestMessageRequest();

        // Assert
        Assert.NotNull(request.CorrelationId);
        Assert.NotEmpty(request.CorrelationId);
        Assert.NotNull(request.Headers);
        Assert.True(request.Timestamp <= DateTime.UtcNow);
        Assert.Null(request.SourceService);
        Assert.Null(request.TargetService);
    }

    [Fact]
    public void MessageRequestBase_WithCorrelationId_ShouldSetCorrelationId()
    {
        // Arrange
        var correlationId = "test-correlation-id";

        // Act
        var request = new TestMessageRequest();
        request.CorrelationId = correlationId;

        // Assert
        Assert.Equal(correlationId, request.CorrelationId);
        Assert.NotNull(request.Headers);
    }

    [Fact]
    public void MessageRequestBase_WithNullCorrelationId_ShouldGenerateNewOne()
    {
        // Act
        var request = new TestMessageRequest();

        // Assert
        Assert.NotNull(request.CorrelationId);
        Assert.NotEmpty(request.CorrelationId);
    }

    [Fact]
    public void MessageRequestBase_ShouldAllowSettingProperties()
    {
        // Arrange
        var request = new TestMessageRequest();
        var sourceService = "TestService";
        var targetService = "TargetService";
        var headers = new Dictionary<string, object> { { "Key", "Value" } };

        // Act
        request.SourceService = sourceService;
        request.TargetService = targetService;
        request.Headers = headers;

        // Assert
        Assert.Equal(sourceService, request.SourceService);
        Assert.Equal(targetService, request.TargetService);
        Assert.Equal(headers, request.Headers);
    }

    [Fact]
    public void MessageRequestBase_WithResponse_ShouldInheritCorrectly()
    {
        // Act
        var request = new TestMessageRequestWithResponse();

        // Assert
        Assert.NotNull(request.CorrelationId);
        Assert.NotNull(request.Headers);
        Assert.True(request is IMessageRequest<TestResponse>);
        Assert.True(request is IBonCommand<TestResponse>);
    }

    [Fact]
    public void MessageEventBase_ShouldInitializeCorrectly()
    {
        // Act
        var @event = new TestMessageEvent();

        // Assert
        Assert.NotNull(@event.CorrelationId);
        Assert.NotEmpty(@event.CorrelationId);
        Assert.NotNull(@event.Headers);
        Assert.True(@event.Timestamp <= DateTime.UtcNow);
        Assert.Null(@event.SourceService);
        Assert.True(@event is IBonEvent);
    }

    [Fact]
    public void MessageEventBase_WithCorrelationId_ShouldSetCorrelationId()
    {
        // Arrange
        var correlationId = "test-event-correlation-id";

        // Act
        var @event = new TestMessageEvent();
        @event.CorrelationId = correlationId;

        // Assert
        Assert.Equal(correlationId, @event.CorrelationId);
        Assert.NotNull(@event.Headers);
    }

    [Fact]
    public void MessageEventBase_ShouldAllowSettingProperties()
    {
        // Arrange
        var @event = new TestMessageEvent();
        var sourceService = "EventService";
        var headers = new Dictionary<string, object> { { "EventType", "Test" } };

        // Act
        @event.SourceService = sourceService;
        @event.Headers = headers;

        // Assert
        Assert.Equal(sourceService, @event.SourceService);
        Assert.Equal(headers, @event.Headers);
    }

    [Fact]
    public void MessageInterfaces_ShouldImplementCorrectBaseInterfaces()
    {
        // Arrange
        var request = new TestMessageRequest();
        var requestWithResponse = new TestMessageRequestWithResponse();
        var @event = new TestMessageEvent();

        // Assert
        Assert.True(request is IMessageRequest);
        Assert.True(request is IBonCommand);
        
        Assert.True(requestWithResponse is IMessageRequest<TestResponse>);
        Assert.True(requestWithResponse is IMessageRequest);
        Assert.True(requestWithResponse is IBonCommand<TestResponse>);
        
        Assert.True(@event is IMessageEvent);
        Assert.True(@event is IBonEvent);
    }

    [Fact]
    public void MessageInterfaces_ShouldSupportGenericConstraints()
    {
        // This test ensures that the interfaces can be used in generic constraints
        Assert.True(IsValidMessageRequest(new TestMessageRequest()));
        Assert.True(IsValidMessageRequestWithResponse(new TestMessageRequestWithResponse()));
        Assert.True(IsValidMessageEvent(new TestMessageEvent()));
    }

    private static bool IsValidMessageRequest<T>(T request) where T : class, IMessageRequest
    {
        return request != null;
    }

    private static bool IsValidMessageRequestWithResponse<T>(T request) where T : class, IMessageRequest<TestResponse>
    {
        return request != null;
    }

    private static bool IsValidMessageEvent<T>(T @event) where T : class, IMessageEvent
    {
        return @event != null;
    }
}
