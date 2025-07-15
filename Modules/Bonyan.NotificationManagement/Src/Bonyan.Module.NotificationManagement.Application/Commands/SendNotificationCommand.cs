using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Abstractions.Types;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public record SendNotificationCommand(
    NotificationChannel Channel,
    string UserId,
    string Title,
    string Message,
    string? Link = null,
    string? Purpose = null,
    string? Context = null) : IBonCommand; 