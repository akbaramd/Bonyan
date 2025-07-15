# Bonyan Notification Management Domain

This module provides a comprehensive notification management system that is self-contained and can be used independently in any project. It follows Domain-Driven Design (DDD) principles and is built on the Bonyan framework.

## Features

- **Multi-channel notifications**: Email, SMS, Push, In-App, Webhook, Slack, Teams, Discord
- **Notification templates**: Reusable templates with variable substitution
- **Priority levels**: Low, Normal, High, Urgent, Critical
- **Status tracking**: Draft, Scheduled, Pending, Sending, Sent, Delivered, Read, Failed, Cancelled, Expired
- **Recipient management**: Support for multiple recipient types and contact information
- **Scheduling**: Future notification scheduling
- **Retry mechanism**: Automatic retry with exponential backoff
- **Rate limiting**: Built-in rate limiting per recipient and notification type
- **Domain events**: Rich event system for integration
- **Audit trail**: Full audit and soft deletion support

## Architecture

### Core Entities

#### BonNotification (Aggregate Root)
The main aggregate root representing a notification in the system.

**Key Features:**
- Factory methods for different notification types
- Recipient management
- Status transitions
- Scheduling capabilities
- Retry mechanism
- Content management
- Metadata support

**Usage Example:**
```csharp
// Create an email notification
var notification = BonNotification.CreateEmail(
    "Welcome to our platform!",
    "Thank you for joining us.",
    NotificationPriority.Normal,
    "Welcome message"
);

// Add recipients
notification.AddRecipient("user123", "User", "John Doe", "john@example.com");

// Schedule for later
notification.Schedule(DateTime.UtcNow.AddHours(1));

// Send immediately
notification.Send();
```

#### NotificationTemplate
Reusable templates for creating notifications with variable substitution.

**Key Features:**
- Template versioning
- Variable support (default and required)
- Multiple content formats (HTML, plain text)
- Category organization
- Active/inactive status

**Usage Example:**
```csharp
// Create an email template
var template = NotificationTemplate.CreateEmailTemplate(
    "Welcome Email",
    "Welcome email for new users",
    "Welcome to {{PlatformName}}!",
    "Dear {{UserName}},<br/>Welcome to {{PlatformName}}!",
    "Dear {{UserName}},\n\nWelcome to {{PlatformName}}!"
);

// Add required variables
template.AddRequiredVariable("UserName");
template.AddRequiredVariable("PlatformName");

// Add default variables
template.AddDefaultVariable("PlatformName", "Our Platform");

// Create content from template
var variables = new Dictionary<string, object>
{
    ["UserName"] = "John Doe"
};
var content = template.CreateContent(variables);
```

#### NotificationRecipient
Represents a recipient of a notification with contact information and status tracking.

**Key Features:**
- Multiple contact methods (email, phone, device token)
- Status tracking (read, delivered, failed)
- Factory methods for different recipient types
- Metadata support

**Usage Example:**
```csharp
// Create different types of recipients
var userRecipient = NotificationRecipient.CreateUser("user123", "John Doe", "john@example.com");
var emailRecipient = NotificationRecipient.CreateEmail("jane@example.com", "Jane Smith");
var smsRecipient = NotificationRecipient.CreateSms("+1234567890", "John Doe");
var pushRecipient = NotificationRecipient.CreatePush("device_token_123", "John Doe", "ios");
```

### Value Objects

#### NotificationContent
Represents the content of a notification with various formats and metadata.

**Features:**
- Multiple content formats (subject, body, HTML, plain text)
- Variable substitution
- Attachment support
- Header management

### Enumerations

#### NotificationType
- Email, Sms, Push, InApp, Webhook, Chat (generic chat channel for Slack/Discord/Teams, etc.)

#### NotificationPriority
- Low, Normal, High, Urgent, Critical

#### NotificationStatus
- Draft, Scheduled, Pending, Sending, Sent, Delivered, Read, Failed, Cancelled, Expired

### Domain Services

#### INotificationDomainService
Provides business logic for notification operations.

**Key Methods:**
- `CanSendNotificationAsync`: Validates if a notification can be sent
- `ValidateNotificationContentAsync`: Validates notification content
- `ShouldRateLimitAsync`: Checks if notification should be rate limited
- `GetOptimalSendTimeAsync`: Gets optimal send time based on recipient preferences
- `ShouldRetryAsync`: Determines if a notification should be retried
- `GetNextRetryDelayAsync`: Calculates next retry delay
- `ValidateRecipientAsync`: Validates recipient information
- `IsNotificationExpiredAsync`: Checks if notification is expired
- `GetRecipientStatisticsAsync`: Gets notification statistics for a recipient

### Domain Events

The module includes a rich set of domain events for integration:

- `NotificationCreatedEvent`: Raised when a notification is created
- `NotificationSentEvent`: Raised when a notification is sent
- `NotificationScheduledEvent`: Raised when a notification is scheduled
- `NotificationReadEvent`: Raised when a notification is read
- `NotificationRecipientAddedEvent`: Raised when a recipient is added
- `NotificationRecipientRemovedEvent`: Raised when a recipient is removed
- `NotificationCancelledEvent`: Raised when a notification is cancelled
- `NotificationRetryEvent`: Raised when a notification is retried
- `NotificationContentUpdatedEvent`: Raised when content is updated
- `NotificationPriorityChangedEvent`: Raised when priority is changed

### Specifications

The module provides a comprehensive set of specifications for querying notifications:

- `ByType`: Filter by notification type
- `ByStatus`: Filter by notification status
- `ByPriority`: Filter by notification priority
- `ByRecipient`: Filter by recipient
- `ByCategory`: Filter by category
- `BySource`: Filter by source
- `CreatedAfter/CreatedBefore`: Filter by creation date
- `ScheduledBetween`: Filter by scheduled date range
- `CanBeSent`: Filter for sendable notifications
- `IsExpired`: Filter for expired notifications
- `NeedsRetry`: Filter for notifications needing retry
- `HighPriority`: Filter for high priority notifications
- `IsRead/IsUnread`: Filter by read status

### Repository Interfaces

#### IBonNotificationRepository
Provides write operations for notifications.

#### IBonNotificationReadOnlyRepository
Provides read-only operations for notifications.

#### IBonNotificationTemplateRepository
Provides write operations for notification templates.

#### IBonNotificationTemplateReadOnlyRepository
Provides read-only operations for notification templates.

### Providers

#### NotificationProvider
Represents a provider implementation (e.g., SendGrid, Mailgun, Twilio) for a given notification type. A single notification type can have multiple providers configured with different priorities.

**Key Features:**
- Activation/deactivation support
- Priority ordering for fallback
- Dynamic configuration (key/value pairs)
- Implementation alias (fully-qualified type name) for DI

**Usage Example:**
```csharp
// Register a new provider
autoProvider = new NotificationProvider(
    "Twilio",
    "twilio",
    NotificationType.Sms,
    typeof(TwilioSmsProvider).AssemblyQualifiedName!,
    priority: 1
);
notificationProviderRepository.AddAsync(autoProvider);
```

### Updated Notification Types
`NotificationType` now contains **global** categories only:
- Email, Sms, Push, InApp, Webhook, Chat (generic chat channel for Slack/Discord/Teams, etc.)

## Usage Examples

### Creating and Sending Notifications

```csharp
// Create a notification
var notification = BonNotification.CreateEmail(
    "Order Confirmation",
    "Your order #12345 has been confirmed.",
    NotificationPriority.High
);

// Add recipients
notification.AddRecipient("user123", "User", "John Doe", "john@example.com");

// Set metadata
notification.SetMetadata("OrderId", "12345");
notification.SetMetadata("Amount", 99.99m);

// Send the notification
notification.Send();
```

### Using Templates

```csharp
// Get template
var template = await templateRepository.GetByNameAsync("OrderConfirmation");

// Create notification from template
var notification = BonNotification.CreateEmail(
    template.Subject,
    template.Body,
    NotificationPriority.Normal,
    templateId: template.Id
);

// Add variables
var variables = new Dictionary<string, object>
{
    ["OrderNumber"] = "12345",
    ["CustomerName"] = "John Doe",
    ["TotalAmount"] = 99.99m
};

// Create content from template
var content = template.CreateContent(variables);
notification.UpdateContent(content);

// Add recipient and send
notification.AddRecipient("user123", "User", "John Doe", "john@example.com");
notification.Send();
```

### Querying Notifications

```csharp
// Get notifications by type
var emailNotifications = await notificationRepository.GetByTypeAsync(NotificationType.Email);

// Get notifications by status
var pendingNotifications = await notificationRepository.GetByStatusAsync(NotificationStatus.Pending);

// Get notifications for a specific recipient
var userNotifications = await notificationRepository.GetByRecipientAsync("user123");

// Get sendable notifications
var sendableNotifications = await notificationRepository.GetSendableNotificationsAsync();

// Get notifications needing retry
var retryNotifications = await notificationRepository.GetNotificationsNeedingRetryAsync();
```

### Using Specifications

```csharp
// Create a complex query using specifications
var specification = new NotificationSpecifications.ByType(NotificationType.Email)
    .And(new NotificationSpecifications.ByStatus(NotificationStatus.Pending))
    .And(new NotificationSpecifications.CreatedAfter(DateTime.UtcNow.AddDays(-7)));

var notifications = await notificationRepository.FindAsync(specification);
```

### Domain Service Usage

```csharp
// Validate if notification can be sent
var canSend = await domainService.CanSendNotificationAsync(notification);

// Check rate limiting
var shouldRateLimit = await domainService.ShouldRateLimitAsync(
    "user123", 
    NotificationType.Email, 
    TimeSpan.FromHours(1)
);

// Get optimal send time
var optimalTime = await domainService.GetOptimalSendTimeAsync("user123", NotificationType.Email);

// Get recipient statistics
var stats = await domainService.GetRecipientStatisticsAsync("user123", TimeSpan.FromDays(30));
```

## Integration

### Adding to Your Project

1. Add the module to your solution
2. Register the module in your application startup:

```csharp
// In your Program.cs or Startup.cs
services.AddModule<BonNotificationManagementDomainModule>();
```

3. Implement the repository interfaces in your infrastructure layer
4. Configure your notification providers (email, SMS, push, etc.)

### Event Handling

```csharp
// Subscribe to domain events
public class NotificationEventHandler : 
    INotificationHandler<NotificationSentEvent>,
    INotificationHandler<NotificationReadEvent>
{
    public async Task Handle(NotificationSentEvent notification, CancellationToken cancellationToken)
    {
        // Handle notification sent event
        await LogNotificationSent(notification.NotificationId);
    }

    public async Task Handle(NotificationReadEvent notification, CancellationToken cancellationToken)
    {
        // Handle notification read event
        await UpdateUserActivity(notification.RecipientId);
    }
}
```

## Best Practices

1. **Use templates** for consistent messaging
2. **Validate recipients** before sending
3. **Handle rate limiting** to avoid spam
4. **Monitor statistics** for engagement insights
5. **Use appropriate priorities** for different notification types
6. **Implement retry logic** for failed notifications
7. **Track delivery status** for important notifications
8. **Use metadata** for additional context
9. **Schedule notifications** during optimal times
10. **Handle domain events** for integration points

## Extensibility

The module is designed to be extensible:

- Add new notification types by extending `NotificationType`
- Add new priority levels by extending `NotificationPriority`
- Add new statuses by extending `NotificationStatus`
- Implement custom domain services for specific business logic
- Add new specifications for complex queries
- Extend value objects for additional functionality

## Dependencies

- `Bonyan.Layer.Domain`: Core domain framework
- `Bonyan.Modularity`: Module system
- `Microsoft.Extensions.DependencyInjection`: Dependency injection

## License

This module is part of the Bonyan framework and follows the same licensing terms. 