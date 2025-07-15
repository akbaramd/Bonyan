using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;
using Bonyan.UnitOfWork;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class DeleteAllNotificationsCommandHandler : IBonCommandHandler<DeleteAllNotificationsCommand>
{
    private readonly IBonNotificationRepository _repository;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;

    public DeleteAllNotificationsCommandHandler(IBonNotificationRepository repository, IBonUnitOfWorkManager unitOfWorkManager)
    {
        _repository = repository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task HandleAsync(DeleteAllNotificationsCommand command, CancellationToken cancellationToken = default)
    {
        using var uow = _unitOfWorkManager.Begin(new BonUnitOfWorkOptions());
        
        try
        {
            var userNotifications = await _repository.FindAsync(
                 n => n.UserId == command.UserId
            );
            
            if (!userNotifications.Any())
            {
                return; // No notifications to delete
            }
            
            foreach (var notification in userNotifications)
            {
                await _repository.DeleteAsync(notification, autoSave: false);
            }
            
            await uow.CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to delete all notifications: {ex.Message}", ex);
        }
    }
} 