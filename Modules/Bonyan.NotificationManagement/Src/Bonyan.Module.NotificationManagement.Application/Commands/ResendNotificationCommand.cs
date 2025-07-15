using System;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Abstractions.Providers;
using Bonyan.Module.NotificationManagement.Abstractions.Types;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class ResendNotificationCommand : IBonCommand
{
    public Guid NotificationId { get; }
    public NotificationChannel Channel { get; }

    public ResendNotificationCommand(Guid notificationId, NotificationChannel channel)
    {
        NotificationId = notificationId;
        Channel = channel;
    }
} 