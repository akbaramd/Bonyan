using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;
using Bonyan.UnitOfWork;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class DeleteReadNotificationsCommandHandler : IBonCommandHandler<DeleteReadNotificationsCommand>
{
    private readonly IBonNotificationRepository _repository;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;

    public DeleteReadNotificationsCommandHandler(IBonNotificationRepository repository, IBonUnitOfWorkManager unitOfWorkManager)
    {
        _repository = repository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task HandleAsync(DeleteReadNotificationsCommand command, CancellationToken cancellationToken = default)
    {
        using var uow = _unitOfWorkManager.Begin(new BonUnitOfWorkOptions());
        
        try
        {
            var readNotifications = await _repository.FindAsync(
                 n => n.UserId == command.UserId && n.IsRead
                
            );
            
            if (!readNotifications.Any())
            {
                return; // No read notifications to delete
            }
            
            foreach (var notification in readNotifications)
            {
                await _repository.DeleteAsync(notification, autoSave: false);
            }
            
            await uow.CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to delete read notifications: {ex.Message}", ex);
        }
    }
} 