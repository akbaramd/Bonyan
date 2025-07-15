using System;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;
using Bonyan.UnitOfWork;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class UpdateNotificationCommandHandler : IBonCommandHandler<UpdateNotificationCommand>
{
    private readonly IBonNotificationRepository _repository;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;

    public UpdateNotificationCommandHandler(IBonNotificationRepository repository, IBonUnitOfWorkManager unitOfWorkManager)
    {
        _repository = repository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task HandleAsync(UpdateNotificationCommand command, CancellationToken cancellationToken = default)
    {
        using var uow = _unitOfWorkManager.Begin(new BonUnitOfWorkOptions());
        
        try
        {
            var notification = await _repository.FindByIdAsync(command.NotificationId);
            if (notification == null)
            {
                throw new InvalidOperationException($"Notification with ID {command.NotificationId} not found");
            }
            
            notification.UpdateMessage(command.Title, command.Message, command.Link);
            await _repository.UpdateAsync(notification, autoSave: false);
            
            await uow.CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to update notification: {ex.Message}", ex);
        }
    }
} 