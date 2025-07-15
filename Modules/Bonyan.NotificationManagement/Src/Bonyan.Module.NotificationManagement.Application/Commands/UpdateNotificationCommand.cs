using System;
using Bonyan.Mediators;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class UpdateNotificationCommand : IBonCommand
{
    public Guid NotificationId { get; }
    public string Title { get; }
    public string Message { get; }
    public string? Link { get; }

    public UpdateNotificationCommand(Guid notificationId, string title, string message, string? link = null)
    {
        NotificationId = notificationId;
        Title = title;
        Message = message;
        Link = link;
    }
} 