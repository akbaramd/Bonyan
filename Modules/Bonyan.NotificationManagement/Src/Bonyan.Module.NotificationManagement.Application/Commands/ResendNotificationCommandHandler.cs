using System;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Abstractions.Providers;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class ResendNotificationCommandHandler : IBonCommandHandler<ResendNotificationCommand>
{
    private readonly INotificationSender _sender;
    private readonly IBonNotificationRepository _repository;

    public ResendNotificationCommandHandler(INotificationSender sender, IBonNotificationRepository repository)
    {
        _sender = sender;
        _repository = repository;
    }

    public async Task HandleAsync(ResendNotificationCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = await _repository.FindByIdAsync(command.NotificationId);
            if (notification == null)
            {
                throw new InvalidOperationException($"Notification with ID {command.NotificationId} not found");
            }
            
            // Resend the notification through providers
            await _sender.SendAsync(
                command.Channel,
                notification.UserId,
                notification.Title,
                notification.Message,
                notification.Link,
                cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to resend notification: {ex.Message}", ex);
        }
    }
} 