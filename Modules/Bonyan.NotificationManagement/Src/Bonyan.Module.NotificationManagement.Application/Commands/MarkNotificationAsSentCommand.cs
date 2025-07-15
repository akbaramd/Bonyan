using System;
using Bonyan.Mediators;

namespace Bonyan.Module.NotificationManagement.Application.Commands;

public record MarkNotificationAsSentCommand(Guid NotificationId) : IBonCommand; 