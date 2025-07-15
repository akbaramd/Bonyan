using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;
using Bonyan.UnitOfWork;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class MarkAllNotificationsAsReadCommandHandler : IBonCommandHandler<MarkAllNotificationsAsReadCommand>
{
    private readonly IBonNotificationRepository _repository;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;

    public MarkAllNotificationsAsReadCommandHandler(IBonNotificationRepository repository, IBonUnitOfWorkManager unitOfWorkManager)
    {
        _repository = repository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task HandleAsync(MarkAllNotificationsAsReadCommand command, CancellationToken cancellationToken = default)
    {
        using var uow = _unitOfWorkManager.Begin(new BonUnitOfWorkOptions());
        
        try
        {
            var unreadNotifications = await _repository.FindAsync(
                predicate: n => n.UserId == command.UserId && !n.IsRead
            );
            
            if (!unreadNotifications.Any())
            {
                return; // No unread notifications to mark
            }
            
            foreach (var notification in unreadNotifications)
            {
                notification.MarkAsRead();
                await _repository.UpdateAsync(notification, autoSave: false);
            }
            
            await uow.CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to mark all notifications as read: {ex.Message}", ex);
        }
    }
} 