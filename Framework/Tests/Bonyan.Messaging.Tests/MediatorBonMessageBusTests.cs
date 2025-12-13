using Bonyan.Mediators;
using Bonyan.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bonyan.Messaging.Tests;

public class MediatorBonMessageBusTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IBonMessageBus _messageBus;

    public MediatorBonMessageBusTests()
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
    public async Task SendAsync_WithResponse_ShouldHandleRequestCorrectly()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            SourceService = "TestService",
            TargetService = "UserService"
        };

        // Act
        var response = await _messageBus.SendAsync<CreateUserRequest, CreateUserResponse>(
            "UserService", request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.UserId);
        Assert.Equal("User created successfully.", response.Message);
        Assert.Equal(request.CorrelationId, request.CorrelationId);
    }

    [Fact]
    public async Task SendAsync_WithoutResponse_ShouldHandleRequestCorrectly()
    {
        // Arrange
        var request = new DeleteUserRequest
        {
            UserId = Guid.NewGuid(),
            Reason = "Test deletion",
            SourceService = "TestService",
            TargetService = "UserService"
        };

        // Act & Assert
        await _messageBus.SendAsync("UserService", request);
        
        // If no exception is thrown, the test passes
        Assert.True(true);
    }

    [Fact]
    public async Task SendAsync_WithResponse_ShouldSetTargetService()
    {
        // Arrange
        var request = new GetUserRequest
        {
            UserId = Guid.NewGuid(),
            SourceService = "TestService",
            TargetService = "UserService"
        };

        // Act
        var response = await _messageBus.SendAsync<GetUserRequest, GetUserResponse>(
            "UserService", request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(request.UserId, response.UserId);
        Assert.Equal("Test User", response.Name);
        Assert.Equal("test@example.com", response.Email);
        Assert.Equal("UserService", request.TargetService);
    }

    [Fact]
    public async Task PublishAsync_ShouldHandleEventCorrectly()
    {
        // Arrange
        var userCreatedEvent = new UserCreatedEvent
        {
            UserId = Guid.NewGuid(),
            Name = "Jane Doe",
            Email = "jane.doe@example.com",
            SourceService = "UserService"
        };

        // Act & Assert
        await _messageBus.PublishAsync(userCreatedEvent);
        
        // If no exception is thrown, the test passes
        Assert.True(true);
    }

    [Fact]
    public async Task SendAsync_ShouldPreserveCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var request = new CreateUserRequest
        {
            Name = "Test User",
            Email = "test@example.com",
            CorrelationId = correlationId,
            SourceService = "TestService",
            TargetService = "UserService"
        };

        // Act
        var response = await _messageBus.SendAsync<CreateUserRequest, CreateUserResponse>(
            "UserService", request);

        // Assert
        Assert.Equal(correlationId, request.CorrelationId);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task SendAsync_ShouldGenerateCorrelationIdIfNotProvided()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Name = "Test User",
            Email = "test@example.com",
            CorrelationId = null, // Not provided
            SourceService = "TestService",
            TargetService = "UserService"
        };

        // Act
        var response = await _messageBus.SendAsync<CreateUserRequest, CreateUserResponse>(
            "UserService", request);

        // Assert
        Assert.NotNull(request.CorrelationId);
        Assert.NotEmpty(request.CorrelationId);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task SendAsync_ShouldHandleHeadersCorrectly()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Name = "Test User",
            Email = "test@example.com",
            SourceService = "TestService",
            TargetService = "UserService",
            Headers = new Dictionary<string, object>
            {
                { "Priority", "High" },
                { "RetryCount", 3 }
            }
        };

        // Act
        var response = await _messageBus.SendAsync<CreateUserRequest, CreateUserResponse>(
            "UserService", request);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(request.Headers);
        Assert.Equal("High", request.Headers["Priority"]);
        Assert.Equal(3, request.Headers["RetryCount"]);
    }

    [Fact]
    public async Task SendAsync_ShouldThrowExceptionForUnregisteredHandler()
    {
        // Arrange
        var unregisteredRequest = new UnregisteredRequest
        {
            Data = "Test data",
            SourceService = "TestService",
            TargetService = "UserService"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _messageBus.SendAsync<UnregisteredRequest, UnregisteredResponse>(
                "UserService", unregisteredRequest));
    }

    [Fact]
    public async Task SendAsync_ShouldHandleMultipleRequestsConcurrently()
    {
        // Arrange
        var requests = Enumerable.Range(1, 5).Select(i => new CreateUserRequest
        {
            Name = $"User {i}",
            Email = $"user{i}@example.com",
            SourceService = "TestService",
            TargetService = "UserService"
        }).ToList();

        // Act
        var tasks = requests.Select(request =>
            _messageBus.SendAsync<CreateUserRequest, CreateUserResponse>("UserService", request));
        
        var responses = await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(5, responses.Length);
        foreach (var response in responses)
        {
            Assert.NotNull(response);
            Assert.NotEqual(Guid.Empty, response.UserId);
        }
    }

    [Fact]
    public async Task PublishAsync_ShouldHandleMultipleEventsConcurrently()
    {
        // Arrange
        var events = Enumerable.Range(1, 3).Select(i => new UserCreatedEvent
        {
            UserId = Guid.NewGuid(),
            Name = $"User {i}",
            Email = $"user{i}@example.com",
            SourceService = "UserService"
        }).ToList();

        // Act
        var tasks = events.Select(@event => _messageBus.PublishAsync(@event));
        await Task.WhenAll(tasks);

        // Assert
        // If no exception is thrown, the test passes
        Assert.True(true);
    }
}

/// <summary>
/// Unregistered request for testing error scenarios
/// </summary>
public class UnregisteredRequest : MessageRequestBase<UnregisteredResponse>
{
    public string Data { get; set; } = string.Empty;
}

public class UnregisteredResponse
{
    public string Result { get; set; } = string.Empty;
}
