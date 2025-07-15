using System;
using Bonyan.Mediators;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public record MarkNotificationAsReadCommand(Guid NotificationId) : IBonCommand; 