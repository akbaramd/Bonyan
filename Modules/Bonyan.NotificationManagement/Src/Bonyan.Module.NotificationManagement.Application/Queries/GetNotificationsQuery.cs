using System.Collections.Generic;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications;

namespace Bonyan.Module.NotificationManagement.Application.Queries;

public record GetNotificationsQuery(
    string UserId,
    bool? IsRead = null,
    int Skip = 0,
    int Take = 20) : IBonQuery<GetNotificationsResult>;

public record GetNotificationsResult(
    IEnumerable<BonNotification> Notifications,
    int TotalCount,
    int Skip,
    int Take); 