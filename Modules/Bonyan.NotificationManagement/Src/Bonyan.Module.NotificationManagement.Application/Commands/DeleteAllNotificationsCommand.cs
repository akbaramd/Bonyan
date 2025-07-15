using Bonyan.Mediators;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class DeleteAllNotificationsCommand : IBonCommand
{
    public string UserId { get; }

    public DeleteAllNotificationsCommand(string userId)
    {
        UserId = userId;
    }
} 