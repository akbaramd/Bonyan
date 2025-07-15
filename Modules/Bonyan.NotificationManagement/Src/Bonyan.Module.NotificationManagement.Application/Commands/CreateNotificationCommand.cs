using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications;


namespace Bonyan.Module.NotificationManagement.Application.Commands;

public record CreateNotificationCommand(string? UserId, string Title, string Message, string? Link, string? Purpose = null, string? Context = null) : IBonCommand<BonNotification>; 