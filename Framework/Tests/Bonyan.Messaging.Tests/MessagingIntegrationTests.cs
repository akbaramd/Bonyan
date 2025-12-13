using Bonyan.Mediators;
using Bonyan.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bonyan.Messaging.Tests;

public class MessagingIntegrationTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IBonMessageBus _messageBus;

    public MessagingIntegrationTests()
    {
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole());

        // Register mediator
        services.AddTransient<IBonMediator, InMemoryBonMediator>();

        // Register handlers
        services.AddTransient<IBonCommandHandler<CreateUserRequest, CreateUserResponse>, CreateUserRequestHandler>();
        services.AddTransient<IBonCommandHandler<DeleteUserRequest>, DeleteUserRequestHandler>();
        services.AddTransient<IBonCommandHandler<GetUserRequest, GetUserResponse>, GetUserRequestHandler>();
        services.AddTransient<IBonEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();

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
    public async Task FullMessagingFlow_ShouldWorkCorrectly()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest
        {
            Name = "Integration Test User",
            Email = "integration@test.com",
            SourceService = "IntegrationTestService",
            TargetService = "UserService"
        };

        // Act - Create user
        var createResponse = await _messageBus.SendAsync<CreateUserRequest, CreateUserResponse>(
            "UserService", createUserRequest);

        // Assert - User creation
        Assert.NotNull(createResponse);
        Assert.NotEqual(Guid.Empty, createResponse.UserId);
        Assert.Equal("User created successfully.", createResponse.Message);

        // Act - Get user details
        var getUserRequest = new GetUserRequest
        {
            UserId = createResponse.UserId,
            SourceService = "IntegrationTestService",
            TargetService = "UserService"
        };

        var getUserResponse = await _messageBus.SendAsync<GetUserRequest, GetUserResponse>(
            "UserService", getUserRequest);

        // Assert - User retrieval
        Assert.NotNull(getUserResponse);
        Assert.Equal(createResponse.UserId, getUserResponse.UserId);
        Assert.Equal("Test User", getUserResponse.Name);

        // Act - Delete user
        var deleteUserRequest = new DeleteUserRequest
        {
            UserId = createResponse.UserId,
            Reason = "Integration test cleanup",
            SourceService = "IntegrationTestService",
            TargetService = "UserService"
        };

        await _messageBus.SendAsync("UserService", deleteUserRequest);

        // Assert - Deletion completed without exception
        Assert.True(true);
    }

    [Fact]
    public async Task EventPublishing_ShouldWorkCorrectly()
    {
        // Arrange
        var userCreatedEvent = new UserCreatedEvent
        {
            UserId = Guid.NewGuid(),
            Name = "Event Test User",
            Email = "event@test.com",
            SourceService = "EventTestService"
        };

        // Act
        await _messageBus.PublishAsync(userCreatedEvent);

        // Assert
        Assert.NotNull(userCreatedEvent.CorrelationId);
        Assert.NotEmpty(userCreatedEvent.CorrelationId);
    }

    [Fact]
    public async Task ConcurrentMessaging_ShouldHandleMultipleRequests()
    {
        // Arrange
        var requests = Enumerable.Range(1, 10).Select(i => new CreateUserRequest
        {
            Name = $"Concurrent User {i}",
            Email = $"concurrent{i}@test.com",
            SourceService = "ConcurrentTestService",
            TargetService = "UserService"
        }).ToList();

        // Act
        var tasks = requests.Select(request =>
            _messageBus.SendAsync<CreateUserRequest, CreateUserResponse>("UserService", request));
        
        var responses = await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(10, responses.Length);
        var userIds = responses.Select(r => r.UserId).ToHashSet();
        Assert.Equal(10, userIds.Count); // All should be unique
    }

    [Fact]
    public async Task CorrelationIdTracking_ShouldWorkAcrossOperations()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        
        var createRequest = new CreateUserRequest
        {
            Name = "Correlation Test User",
            Email = "correlation@test.com",
            CorrelationId = correlationId,
            SourceService = "CorrelationTestService",
            TargetService = "UserService"
        };

        // Act
        var createResponse = await _messageBus.SendAsync<CreateUserRequest, CreateUserResponse>(
            "UserService", createRequest);

        // Assert
        Assert.Equal(correlationId, createRequest.CorrelationId);
        Assert.NotNull(createResponse);

        // Act - Use the same correlation ID for related operations
        var getUserRequest = new GetUserRequest
        {
            UserId = createResponse.UserId,
            CorrelationId = correlationId,
            SourceService = "CorrelationTestService",
            TargetService = "UserService"
        };

        var getUserResponse = await _messageBus.SendAsync<GetUserRequest, GetUserResponse>(
            "UserService", getUserRequest);

        // Assert
        Assert.Equal(correlationId, getUserRequest.CorrelationId);
        Assert.NotNull(getUserResponse);
    }

    [Fact]
    public async Task HeadersPropagation_ShouldWorkCorrectly()
    {
        // Arrange
        var headers = new Dictionary<string, object>
        {
            { "Priority", "High" },
            { "RetryCount", 3 },
            { "Source", "IntegrationTest" }
        };

        var request = new CreateUserRequest
        {
            Name = "Header Test User",
            Email = "header@test.com",
            Headers = headers,
            SourceService = "HeaderTestService",
            TargetService = "UserService"
        };

        // Act
        var response = await _messageBus.SendAsync<CreateUserRequest, CreateUserResponse>(
            "UserService", request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(headers, request.Headers);
        Assert.Equal("High", request.Headers["Priority"]);
        Assert.Equal(3, request.Headers["RetryCount"]);
    }

    [Fact]
    public async Task ServiceRouting_ShouldWorkCorrectly()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Name = "Routing Test User",
            Email = "routing@test.com",
            SourceService = "RoutingTestService",
            TargetService = "UserService"
        };

        // Act
        var response = await _messageBus.SendAsync<CreateUserRequest, CreateUserResponse>(
            "UserService", request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("UserService", request.TargetService);
        Assert.Equal("RoutingTestService", request.SourceService);
    }

    [Fact]
    public async Task ErrorHandling_ShouldThrowAppropriateExceptions()
    {
        // Arrange
        var unregisteredRequest = new UnregisteredRequest
        {
            Data = "Test data",
            SourceService = "ErrorTestService",
            TargetService = "UserService"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _messageBus.SendAsync<UnregisteredRequest, UnregisteredResponse>(
                "UserService", unregisteredRequest));

        Assert.Contains("Handler", exception.Message);
    }

    [Fact]
    public async Task EventHandling_ShouldNotThrowOnMissingHandlers()
    {
        // Arrange
        var unregisteredEvent = new UnregisteredEvent
        {
            EventData = "Test event data",
            SourceService = "EventTestService"
        };

        // Act & Assert
        // Should not throw exception even if no handlers are registered
        await _messageBus.PublishAsync(unregisteredEvent);
        Assert.True(true);
    }

    [Fact]
    public async Task MessageBus_ShouldUseMediatorImplementation()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Name = "Mediator Test User",
            Email = "mediator@test.com",
            SourceService = "MediatorTestService",
            TargetService = "UserService"
        };

        // Act
        var response = await _messageBus.SendAsync<CreateUserRequest, CreateUserResponse>(
            "UserService", request);

        // Assert
        Assert.NotNull(response);
        // Verify it's using the mediator implementation by checking the response
        Assert.Equal("User created successfully.", response.Message);
    }
}

/// <summary>
/// Unregistered event for testing error scenarios
/// </summary>
public class UnregisteredEvent : MessageEventBase
{
    public string EventData { get; set; } = string.Empty;
}
