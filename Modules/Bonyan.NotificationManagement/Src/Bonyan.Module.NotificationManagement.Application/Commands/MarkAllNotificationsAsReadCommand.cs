using Bonyan.Mediators;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class MarkAllNotificationsAsReadCommand : IBonCommand
{
    public string UserId { get; }

    public MarkAllNotificationsAsReadCommand(string userId)
    {
        UserId = userId;
    }
} 