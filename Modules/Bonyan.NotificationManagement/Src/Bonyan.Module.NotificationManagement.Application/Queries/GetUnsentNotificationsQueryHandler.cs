using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;

namespace Bonyan.Module.NotificationManagement.Application.Queries;

public class GetUnsentNotificationsQueryHandler : IBonQueryHandler<GetUnsentNotificationsQuery, IEnumerable<BonNotification>>
{
    private readonly IBonNotificationReadOnlyRepository _repository;

    public GetUnsentNotificationsQueryHandler(IBonNotificationReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BonNotification>> HandleAsync(GetUnsentNotificationsQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetUnsentAsync();
    }
} 