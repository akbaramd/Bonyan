using System;
using Bonyan.Mediators;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class DeleteNotificationCommand : IBonCommand
{
    public Guid NotificationId { get; }

    public DeleteNotificationCommand(Guid notificationId)
    {
        NotificationId = notificationId;
    }
} 