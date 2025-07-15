using System;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;
using Bonyan.UnitOfWork;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class DeleteNotificationCommandHandler : IBonCommandHandler<DeleteNotificationCommand>
{
    private readonly IBonNotificationRepository _repository;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;

    public DeleteNotificationCommandHandler(IBonNotificationRepository repository, IBonUnitOfWorkManager unitOfWorkManager)
    {
        _repository = repository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task HandleAsync(DeleteNotificationCommand command, CancellationToken cancellationToken = default)
    {
        using var uow = _unitOfWorkManager.Begin(new BonUnitOfWorkOptions());
        
        try
        {
            var notification = await _repository.FindByIdAsync(command.NotificationId);
            if (notification == null)
            {
                throw new InvalidOperationException($"Notification with ID {command.NotificationId} not found");
            }
            
            await _repository.DeleteAsync(notification, autoSave: false);
            
            await uow.CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to delete notification: {ex.Message}", ex);
        }
    }
} 