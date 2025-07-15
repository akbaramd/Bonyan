using System;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;
using Bonyan.UnitOfWork;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class CreateNotificationCommandHandler : IBonCommandHandler<CreateNotificationCommand, BonNotification>
{
    private readonly IBonNotificationRepository _repository;
    private readonly IBonUnitOfWorkManager _unitOfWorkManager;

    public CreateNotificationCommandHandler(IBonNotificationRepository repository, IBonUnitOfWorkManager unitOfWorkManager)
    {
        _repository = repository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<BonNotification> HandleAsync(CreateNotificationCommand command, CancellationToken cancellationToken = default)
    {
        using var uow = _unitOfWorkManager.Begin(new BonUnitOfWorkOptions());
        
        try
        {
            var notification = new BonNotification(command.UserId, command.Title, command.Message, command.Link, command.Purpose, command.Context);
            await _repository.AddAsync(notification, autoSave: false);
            
            await uow.CompleteAsync(cancellationToken);
            return notification;
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to create notification: {ex.Message}", ex);
        }
    }
} 