using System;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Abstractions.Providers;
using Bonyan.Module.NotificationManagement.Domain.Notifications;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;
using Bonyan.UnitOfWork;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class SendNotificationCommandHandler : IBonCommandHandler<SendNotificationCommand>
{
    private readonly INotificationSender _sender;
    private readonly IBonMediator _mediator;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;
    private readonly IBonNotificationRepository _notificationRepository;

    public SendNotificationCommandHandler(
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

    public async Task HandleAsync(SendNotificationCommand command, CancellationToken cancellationToken = default)
    {
        BonNotification? notification = null;
        
        // Step 1: Create and save the notification
        using (var saveUow = _unitOfWorkManager.Begin(new BonUnitOfWorkOptions()))
        {
            try
            {
                notification = await _mediator.SendAsync(new CreateNotificationCommand(
                    command.UserId,
                    command.Title,
                    command.Message,
                    command.Link,
                    command.Purpose,
                    command.Context
                ), cancellationToken);
                
                await saveUow.CompleteAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await saveUow.RollbackAsync(cancellationToken);
                throw new InvalidOperationException($"Failed to create and save notification: {ex.Message}", ex);
            }
        }
        
        // Step 2: Send notification through providers (only if save was successful)
        if (notification != null)
        {
            try
            {
                await _sender.SendAsync(
                    command.Channel,
                    command.UserId,
                    command.Title,
                    command.Message,
                    command.Link,
                    cancellationToken);
                
                // Step 3: Mark notification as sent
                using (var sentUow = _unitOfWorkManager.Begin(new BonUnitOfWorkOptions()))
                {
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
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't rollback the saved notification
                // The notification is already saved, so we just log the sending failure
                throw new InvalidOperationException($"Notification saved successfully but failed to send: {ex.Message}", ex);
            }
        }
    }
} 