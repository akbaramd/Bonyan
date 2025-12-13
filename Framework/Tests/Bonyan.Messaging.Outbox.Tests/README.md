# Bonyan Messaging Outbox Tests

This test project demonstrates the proper integration of Bonyan messaging outbox pattern with `BonyanApplication` for comprehensive testing scenarios.

## Architecture

### Test Base Class (`BonyanTestBase<TModule>`)

The `BonyanTestBase<TModule>` provides a foundation for all tests that require a fully initialized Bonyan application:

- **Application Lifecycle Management**: Properly starts and stops the Bonyan application
- **Hosted Services**: Automatically manages hosted services lifecycle
- **Service Resolution**: Provides convenient methods to access services from the DI container
- **Resource Disposal**: Implements proper disposal pattern to clean up resources

### Key Features

1. **Full Application Initialization**: Uses `BonyanApplication.CreateModularBuilder<TModule>()` to create a complete application
2. **Module Configuration**: Supports custom module configuration through `BonConfigurationContext`
3. **Hosted Services**: Automatically starts and stops hosted services like `OutboxProcessorHostedService`
4. **Error Handling**: Graceful error handling during initialization and disposal
5. **Resource Management**: Proper disposal of applications, service providers, and hosted services

## Test Classes

### `BonMessageOutboxTest`
Tests the core outbox functionality with a fully initialized application:
- Message publishing to outbox
- Command sending to outbox
- Integration with hosted services

### `OutboxPatternTests`
Comprehensive tests for the outbox pattern implementation:
- Unit tests for `BonOutboxMessage` creation
- Integration tests for `BonOutBoxMessageBox`
- Mock-based tests for error scenarios

### `IntegrationTests`
End-to-end integration tests demonstrating:
- Complete outbox workflow with hosted services
- Application lifecycle management
- Error handling scenarios
- Multiple test instance isolation

## Module Configuration

### `OutboxRabbitMQModule`
Test-specific module that:
- Configures RabbitMQ for test environment
- Sets up in-memory outbox store
- Configures test service manager
- Provides proper timeout and connection settings

## Usage Example

```csharp
public class MyTest : BonyanTestBase<OutboxRabbitMQModule>
{
    [Fact]
    public async Task MyTest_ShouldWork()
    {
        // Initialize the application with custom configuration
        await InitializeApplicationAsync(
            serviceName: "MyTestService",
            configureServices: context =>
            {
                // Add custom services or overrides
                context.Services.AddSingleton<IMyService, MyTestService>();
            });

        // Access services through the base class
        var messageBus = GetService<IBonMessageBus>();
        var outboxStore = GetService<IOutboxStore>();

        // Perform test operations
        await messageBus.PublishAsync(new TestEvent());

        // Assert results
        var messages = await outboxStore.GetPendingMessagesAsync();
        Assert.Single(messages);
    }
}
```

## Benefits

1. **Realistic Testing**: Tests run against a fully initialized application, not just isolated components
2. **Proper Lifecycle**: Applications are properly started and stopped, including hosted services
3. **Resource Management**: Automatic cleanup prevents resource leaks
4. **Integration Testing**: Tests can verify the complete workflow from message publishing to processing
5. **Maintainable**: Consistent pattern across all tests makes them easier to maintain and understand

## Running Tests

All tests can be run using standard xUnit test runners. The tests will:
1. Initialize a complete Bonyan application
2. Start all hosted services
3. Execute the test logic
4. Stop hosted services and dispose of resources

This ensures that tests run in an environment that closely resembles production, providing higher confidence in the system's behavior.
