using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;

namespace Bonyan.Module.NotificationManagement.Application.Queries;

public class GetNotificationsByPurposeQueryHandler : IBonQueryHandler<GetNotificationsByPurposeQuery, IEnumerable<BonNotification>>
{
    private readonly IBonNotificationReadOnlyRepository _repository;

    public GetNotificationsByPurposeQueryHandler(IBonNotificationReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BonNotification>> HandleAsync(GetNotificationsByPurposeQuery query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(query.UserId))
        {
            return await _repository.GetByPurposeAsync(query.Purpose);
        }
        
        return await _repository.GetByPurposeAsync(query.UserId, query.Purpose);
    }
} 