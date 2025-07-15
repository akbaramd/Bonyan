using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Abstractions.Providers;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;
using Bonyan.UnitOfWork;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class BulkSendNotificationCommandHandler : IBonCommandHandler<BulkSendNotificationCommand>
{
    private readonly INotificationSender _sender;
    private readonly IBonMediator _mediator;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;
    private readonly IBonNotificationRepository _notificationRepository;

    public BulkSendNotificationCommandHandler(
        INotificationSender sender, 
        IBonMediator mediator, 
        IBonUnitOfWorkManager unitOfWorkManager,
        IBonNotificationRepository notificationRepository)
    {
        _sender = sender;
        _mediator = mediator;
        _unitOfWorkManager = unitOfWorkManager;
        _notificationRepository = notificationRepository;
    }

    public async Task HandleAsync(BulkSendNotificationCommand command, CancellationToken cancellationToken = default)
    {
        var failedUsers = new List<string>();
        var successfulUsers = new List<string>();
        
        // Process each user individually to ensure partial success handling
        foreach (var userId in command.UserIds)
        {
            try
            {
                using var uow = _unitOfWorkManager.Begin(new BonUnitOfWorkOptions());
                
                // Create and save notification for this user
                var notification = await _mediator.SendAsync(new CreateNotificationCommand(
                    userId,
                    command.Title,
                    command.Message,
                    command.Link,
                    command.Purpose,
                    command.Context
                ), cancellationToken);
                
                await uow.CompleteAsync(cancellationToken);
                
                // Send notification through providers
                await _sender.SendAsync(
                    command.Channel,
                    userId,
                    command.Title,
                    command.Message,
                    command.Link,
                    cancellationToken);
                
                // Mark notification as sent
                using var sentUow = _unitOfWorkManager.Begin(new BonUnitOfWorkOptions());
                try
                {
                    await _notificationRepository.MarkAsSentAsync(notification.Id);
                    await sentUow.CompleteAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    await sentUow.RollbackAsync(cancellationToken);
                    // Log the error but don't fail the entire operation
                    // The notification was sent successfully, just couldn't mark it as sent
                }
                
                successfulUsers.Add(userId);
            }
            catch (Exception ex)
            {
                failedUsers.Add(userId);
                // Log the error for this specific user but continue with others
                // You might want to add proper logging here
            }
        }
        
        // If all users failed, throw an exception
        if (successfulUsers.Count == 0 && failedUsers.Count > 0)
        {
            throw new InvalidOperationException($"Failed to send notifications to all users: {string.Join(", ", failedUsers)}");
        }
        
        // If some users failed, you might want to log this or handle it differently
        if (failedUsers.Count > 0)
        {
            // You could throw a partial success exception or log the failures
            // For now, we'll just continue as some notifications were sent successfully
        }
    }
} 