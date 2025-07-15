using System;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications;

namespace Bonyan.Module.NotificationManagement.Application.Queries;

public record GetNotificationByIdQuery(Guid NotificationId) : IBonQuery<BonNotification?>; 