using Bonyan.Mediators;
using Bonyan.Messaging.Abstractions;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bonyan.Messaging.Tests;

/// <summary>
/// Test messages implementing the new messaging interfaces
/// </summary>
public class CreateUserRequest : MessageRequestBase<CreateUserResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class CreateUserResponse
{
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class DeleteUserRequest : MessageRequestBase
{
    public Guid UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class UserCreatedEvent : MessageEventBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class GetUserRequest : MessageRequestBase<GetUserResponse>
{
    public Guid UserId { get; set; }
}

public class GetUserResponse
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Test handlers for the new messaging interfaces
/// </summary>
public class CreateUserRequestHandler : IBonCommandHandler<CreateUserRequest, CreateUserResponse>
{
    private readonly ILogger<CreateUserRequestHandler> _logger;
    private readonly IBonMessageBus _messageBus;

    public CreateUserRequestHandler(ILogger<CreateUserRequestHandler> logger, IBonMessageBus messageBus)
    {
        _logger = logger;
        _messageBus = messageBus;
    }

    public async Task<CreateUserResponse> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user {Name} with email {Email}. CorrelationId: {CorrelationId}",
            request.Name, request.Email, request.CorrelationId);

        var userId = Guid.NewGuid();
        
        // Simulate user creation logic
        await Task.Delay(10, cancellationToken);

        // Publish UserCreatedEvent
        await _messageBus.PublishAsync(new UserCreatedEvent
        {
            UserId = userId,
            Name = request.Name,
            Email = request.Email,
            CorrelationId = request.CorrelationId,
            SourceService = "UserService"
        }, cancellationToken);

        return new CreateUserResponse { UserId = userId, Message = "User created successfully." };
    }
}

public class DeleteUserRequestHandler : IBonCommandHandler<DeleteUserRequest>
{
    private readonly ILogger<DeleteUserRequestHandler> _logger;

    public DeleteUserRequestHandler(ILogger<DeleteUserRequestHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(DeleteUserRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting user {UserId} due to {Reason}. CorrelationId: {CorrelationId}",
            request.UserId, request.Reason, request.CorrelationId);
        
        // Simulate user deletion logic
        await Task.Delay(5, cancellationToken);
    }
}

public class GetUserRequestHandler : IBonCommandHandler<GetUserRequest, GetUserResponse>
{
    private readonly ILogger<GetUserRequestHandler> _logger;

    public GetUserRequestHandler(ILogger<GetUserRequestHandler> logger)
    {
        _logger = logger;
    }

    public async Task<GetUserResponse> HandleAsync(GetUserRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting user {UserId}. CorrelationId: {CorrelationId}",
            request.UserId, request.CorrelationId);
        
        // Simulate fetching user from database
        await Task.Delay(5, cancellationToken);

        return new GetUserResponse
        {
            UserId = request.UserId,
            Name = "Test User",
            Email = "test@example.com"
        };
    }
}

public class UserCreatedEventHandler : IBonEventHandler<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(UserCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received UserCreatedEvent for user {UserId} ({Name}). CorrelationId: {CorrelationId}",
            @event.UserId, @event.Name, @event.CorrelationId);
        
        // Simulate further processing
        await Task.Delay(2, cancellationToken);
    }
}

/// <summary>
/// Test module for messaging tests
/// </summary>
public class MessagingTestModule : BonModule
{
    public MessagingTestModule()
    {
        DependOn<Bonyan.Messaging.BonMessagingModule>();
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
    {
        // Register logging
        context.Services.AddLogging(configure => configure.AddConsole());

       

        return ValueTask.CompletedTask;
    }
}
