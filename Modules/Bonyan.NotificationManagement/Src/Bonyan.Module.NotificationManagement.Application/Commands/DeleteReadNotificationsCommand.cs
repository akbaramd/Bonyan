using Bonyan.Mediators;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public class DeleteReadNotificationsCommand : IBonCommand
{
    public string UserId { get; }

    public DeleteReadNotificationsCommand(string userId)
    {
        UserId = userId;
    }
} 