using System.Collections.Generic;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications;

namespace Bonyan.Module.NotificationManagement.Application.Queries;

public record GetUnsentNotificationsQuery : IBonCommand<IEnumerable<BonNotification>>; 