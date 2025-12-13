using Bonyan.Layer.Domain;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.OutBox;
using FluentAssertions;
using Moq;
using Xunit;

namespace Bonyan.Messaging.OutBox.Tests;

/// <summary>
/// Tests for the outbox pattern implementation.
/// </summary>
public class OutboxPatternTests
{
    [Fact]
    public void BonOutboxMessage_Create_ShouldSetAllProperties()
    {
        // Arrange
        var destination = "TestService";
        var payload = "{\"test\": \"data\"}";
        var messageType = "TestMessage";
        var headers = "{\"header1\": \"value1\"}";
        var replyQueueName = "reply-queue";
        var correlationId = "test-correlation-id";

        // Act
        var message = BonOutboxMessage.Create(destination, payload, messageType, headers, replyQueueName, correlationId);

        // Assert
        message.Destination.Should().Be(destination);
        message.Payload.Should().Be(payload);
        message.MessageType.Should().Be(messageType);
        message.Headers.Should().Be(headers);
        message.ReplyQueueName.Should().Be(replyQueueName);
        message.CorrelationId.Should().Be(correlationId);
        message.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void BonOutboxMessage_Create_WithNullReplyQueue_ShouldUseEmptyString()
    {
        // Arrange
        var destination = "TestService";
        var payload = "{\"test\": \"data\"}";
        var messageType = "TestMessage";
        var headers = "{\"header1\": \"value1\"}";

        // Act
        var message = BonOutboxMessage.Create(destination, payload, messageType, headers, null);

        // Assert
        message.ReplyQueueName.Should().Be(string.Empty);
    }

    [Fact]
    public void BonOutboxMessage_Create_WithNullCorrelationId_ShouldGenerateNewId()
    {
        // Arrange
        var destination = "TestService";
        var payload = "{\"test\": \"data\"}";
        var messageType = "TestMessage";
        var headers = "{\"header1\": \"value1\"}";

        // Act
        var message = BonOutboxMessage.Create(destination, payload, messageType, headers, "reply-queue", null);

        // Assert
        message.CorrelationId.Should().NotBeNullOrEmpty();
        Guid.TryParse(message.CorrelationId, out _).Should().BeTrue();
    }

    [Fact]
    public void InMemoryOutboxStore_AddAsync_ShouldStoreMessage()
    {
        // Arrange
        var store = new InMemoryOutboxStore();
        var message = BonOutboxMessage.Create("TestService", "payload", "TestMessage", "headers", "reply");

        // Act
        var result = store.AddAsync(message);

        // Assert
        result.IsCompletedSuccessfully.Should().BeTrue();
    }

    [Fact]
    public async Task InMemoryOutboxStore_GetPendingMessagesAsync_ShouldReturnStoredMessages()
    {
        // Arrange
        var store = new InMemoryOutboxStore();
        var message1 = BonOutboxMessage.Create("Service1", "payload1", "Message1", "headers1", "reply1");
        var message2 = BonOutboxMessage.Create("Service2", "payload2", "Message2", "headers2", "reply2");

        await store.AddAsync(message1);
        await store.AddAsync(message2);

        // Act
        var messages = await store.GetPendingMessagesAsync();

        // Assert
        messages.Should().HaveCount(2);
        messages.Should().Contain(m => m.Destination == "Service1");
        messages.Should().Contain(m => m.Destination == "Service2");
    }

    [Fact]
    public async Task InMemoryOutboxStore_DeleteAsync_ShouldRemoveMessage()
    {
        // Arrange
        var store = new InMemoryOutboxStore();
        var message = BonOutboxMessage.Create("TestService", "payload", "TestMessage", "headers", "reply");
        await store.AddAsync(message);

        // Act
        await store.DeleteAsync(message.Id);

        // Assert
        var messages = await store.GetPendingMessagesAsync();
        messages.Should().BeEmpty();
    }

    [Fact]
    public void BonOutBoxMessageBox_Constructor_ShouldThrowOnNullDependencies()
    {
        // Arrange
        var mockOutboxStore = new Mock<IOutboxStore>();
        var mockProducer = new Mock<IBonMessageProducer>();
        var mockServiceManager = new Mock<BonServiceManager>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BonOutBoxMessageBox(null!, mockProducer.Object, mockServiceManager.Object));
        Assert.Throws<ArgumentNullException>(() => new BonOutBoxMessageBox(mockOutboxStore.Object, null!, mockServiceManager.Object));
        Assert.Throws<ArgumentNullException>(() => new BonOutBoxMessageBox(mockOutboxStore.Object, mockProducer.Object, null!));
    }

   
  
    [Fact]
    public async Task BonOutBoxMessageBox_SendAsync_WithResponse_ShouldThrowNotImplementedException()
    {
        // Arrange
        var mockOutboxStore = new Mock<IOutboxStore>();
        var mockProducer = new Mock<IBonMessageProducer>();
        var mockServiceManager = new Mock<BonServiceManager>();

        var messageBox = new BonOutBoxMessageBox(mockOutboxStore.Object, mockProducer.Object, mockServiceManager.Object);
        var request = new TestMessageRequestWithResponse { CorrelationId = "test-correlation" };

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => 
            messageBox.SendAsync<TestMessageRequestWithResponse, TestMessageResponse>("DestinationService", request));
    }
}

// Test message classes
public class TestMessageRequest : IMessageRequest
{
    public string CorrelationId { get; set; } = string.Empty;
    public IDictionary<string, object>? Headers { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? SourceService { get; set; }
    public string? TargetService { get; set; }
}

public class TestMessageRequestWithResponse : IMessageRequest<TestMessageResponse>
{
    public string CorrelationId { get; set; } = string.Empty;
    public IDictionary<string, object>? Headers { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? SourceService { get; set; }
    public string? TargetService { get; set; }
}

public class TestMessageResponse
{
    public string Result { get; set; } = string.Empty;
}

public class TestMessageEvent : IMessageEvent
{
    public string CorrelationId { get; set; } = string.Empty;
    public IDictionary<string, object>? Headers { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? SourceService { get; set; }
}
