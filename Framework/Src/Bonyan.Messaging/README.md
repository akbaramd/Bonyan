# Bonyan Messaging Architecture

## Overview

The Bonyan Messaging system provides a flexible, mediator-based messaging architecture that supports both in-memory and distributed messaging patterns. The system is designed with clean architecture principles, following SOLID design patterns and providing seamless integration between different messaging implementations.

## Architecture Components

### 1. Core Abstractions (`Bonyan.Messaging`)

- **`IBonMessageBus`**: Core messaging interface providing send/publish operations
- **`IMessageEvent`**: Interface for message events that inherit from `IBonEvent`
- **`MessageEventBase`**: Base implementation providing common event functionality
- **`IBonMessageProducer`**: Interface for message production
- **`IBonMessageSubscriber`**: Interface for message subscription
- **`IBonMessageConsumer`**: Interface for message consumption

### 2. Mediator Integration (`Bonyan.Mediator`)

- **`IBonMediator`**: Mediator pattern implementation for in-memory messaging
- **`IBonCommand`**: Interface for commands (fire-and-forget operations)
- **`IBonCommand<TResponse>`**: Interface for commands with responses (replaces both commands and queries)
- **`IBonEvent`**: Interface for events (publish-subscribe operations)

### 3. RabbitMQ Implementation (`Bonyan.Messaging.RabbitMQ`)

- **`IBonRabbitMqBus`**: RabbitMQ-specific message bus implementation
- **`RabbitMqBus`**: Concrete implementation using RabbitMQ
- **`RabbitMqProducer`**: RabbitMQ message producer
- **`RabbitMqSubscriber`**: RabbitMQ message subscriber

## Design Patterns and SOLID Principles

### Single Responsibility Principle (SRP)
- Each component has a single, well-defined responsibility
- `MediatorBonMessageBus` handles mediator-to-message-bus adaptation
- `RabbitMqBus` handles RabbitMQ-specific messaging operations
- Handler classes focus solely on processing specific message types

### Open/Closed Principle (OCP)
- The system is open for extension through new message bus implementations
- Closed for modification through stable interfaces
- New messaging providers can be added without changing existing code

### Liskov Substitution Principle (LSP)
- All implementations of `IBonMessageBus` are fully substitutable
- Mediator and RabbitMQ implementations can be used interchangeably
- Interface contracts are maintained across all implementations

### Interface Segregation Principle (ISP)
- Interfaces are focused and specific to their use cases
- `IBonCommand` and `IBonEvent` are separate interfaces
- Clients depend only on the interfaces they actually use

### Dependency Inversion Principle (DIP)
- High-level modules depend on abstractions, not concretions
- Service registration uses factory patterns for conditional implementations
- Dependencies are injected through constructor injection

## Message Event Interface

### IMessageEvent Interface

The `IMessageEvent` interface extends `IBonEvent` to provide messaging-specific capabilities:

```csharp
public interface IMessageEvent : IBonEvent
{
    string? CorrelationId { get; set; }
    IDictionary<string, object>? Headers { get; set; }
    DateTime Timestamp { get; set; }
    string? SourceService { get; set; }
}
```

### MessageEventBase Class

For convenience, use the `MessageEventBase` abstract class:

```csharp
public class UserCreatedEvent : MessageEventBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

## Usage Patterns

### 1. Default Mediator-Based Messaging

When using only the core messaging and mediator packages:

```csharp
// Service registration
context.AddMediator();
context.AddMessaging(config => 
{
    // Configuration options
});

// Usage
public class UserService
{
    private readonly IBonMessageBus _messageBus;

    public UserService(IBonMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task<CreateUserResponse> CreateUserAsync(CreateUserCommand command)
    {
        // This will use MediatorBonMessageBus internally
        return await _messageBus.SendAsync<CreateUserCommand, CreateUserResponse>(
            "UserService", command);
    }

    public async Task PublishUserCreatedEventAsync(UserCreatedEvent @event)
    {
        // This will use MediatorBonMessageBus internally
        await _messageBus.PublishAsync(@event);
    }
}
```

### 2. RabbitMQ-Based Messaging

When using RabbitMQ for distributed messaging:

```csharp
// Service registration
context.AddMediator();
context.AddMessaging(config => 
{
    config.AddRabbitMq(rabbitConfig =>
    {
        rabbitConfig.HostName = "localhost";
        rabbitConfig.Port = 5672;
        rabbitConfig.UserName = "guest";
        rabbitConfig.Password = "guest";
    });
});

// Usage remains the same - the system automatically switches to RabbitMQ
public class UserService
{
    private readonly IBonMessageBus _messageBus;

    public UserService(IBonMessageBus messageBus)
    {
        _messageBus = messageBus; // This will be RabbitMqBus when RabbitMQ is configured
    }

    public async Task<CreateUserResponse> CreateUserAsync(CreateUserCommand command)
    {
        // This will use RabbitMqBus internally
        return await _messageBus.SendAsync<CreateUserCommand, CreateUserResponse>(
            "UserService", command);
    }
}
```

## Message Handler Registration

### Command Handlers
```csharp
public class CreateUserCommandHandler : IBonCommandHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        // Handle the command
        return new CreateUserResponse { /* ... */ };
    }
}
```

### Command Handlers (with Response)
```csharp
public class GetUserCommandHandler : IBonCommandHandler<GetUserCommand, GetUserResponse>
{
    public async Task<GetUserResponse> HandleAsync(GetUserCommand command, CancellationToken cancellationToken = default)
    {
        // Handle the command and return response
        return new GetUserResponse { /* ... */ };
    }
}
```

### Event Handlers
```csharp
public class UserCreatedEventHandler : IBonEventHandler<UserCreatedEvent>
{
    public async Task HandleAsync(UserCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Handle the event
    }
}
```

## Conditional Implementation Selection

The system automatically selects the appropriate message bus implementation based on available services:

1. **RabbitMQ Available**: Uses `RabbitMqBus` for distributed messaging
2. **Mediator Only**: Uses `MediatorBonMessageBus` for in-memory messaging
3. **Fallback**: Throws descriptive exception if no implementation is available

## Error Handling and Logging

- Comprehensive logging at all levels (Debug, Information, Warning, Error)
- Structured logging with correlation IDs and message types
- Graceful error handling with meaningful exception messages
- Health check integration for monitoring

## Performance Considerations

- **Mediator Mode**: In-memory operations with minimal overhead
- **RabbitMQ Mode**: Distributed operations with network latency considerations
- **Connection Pooling**: Efficient RabbitMQ connection management
- **Async/Await**: Non-blocking operations throughout the system

## Testing Strategy

### Unit Testing
- Test individual handlers in isolation
- Mock dependencies using interfaces
- Verify handler logic and error conditions

### Integration Testing
- Test message bus implementations
- Verify mediator pattern behavior
- Test RabbitMQ integration scenarios

### End-to-End Testing
- Test complete message flows
- Verify distributed messaging scenarios
- Test error handling and recovery

## Migration and Extension Points

### Adding New Message Bus Implementations
1. Implement `IBonMessageBus` interface
2. Register in service collection with conditional logic
3. Update service registration to check for new implementation

### Custom Message Types
1. Implement appropriate mediator interfaces (`IBonCommand`, `IBonEvent`)
2. Create corresponding handlers
3. Register handlers in dependency injection container

### Behavior Customization
1. Implement `IBonMediatorBehavior<TRequest, TResponse>` for cross-cutting concerns
2. Register behaviors in service collection
3. Behaviors will be applied automatically by the mediator

## Best Practices

1. **Message Design**: Keep messages focused and cohesive
2. **Handler Registration**: Register handlers as transient services
3. **Error Handling**: Implement proper exception handling in handlers
4. **Logging**: Use structured logging with appropriate levels
5. **Testing**: Write comprehensive tests for all handlers
6. **Performance**: Consider performance implications of message serialization
7. **Security**: Implement proper authentication and authorization in handlers
