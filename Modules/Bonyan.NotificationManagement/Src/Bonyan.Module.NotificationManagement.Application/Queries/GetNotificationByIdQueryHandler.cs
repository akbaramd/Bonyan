using System.Threading;
using System.Threading.Tasks;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;

namespace Bonyan.Module.NotificationManagement.Application.Queries;

public class GetNotificationByIdQueryHandler : IBonQueryHandler<GetNotificationByIdQuery, BonNotification?>
{
    private readonly IBonNotificationReadOnlyRepository _repository;

    public GetNotificationByIdQueryHandler(IBonNotificationReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<BonNotification?> HandleAsync(GetNotificationByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.FindByIdAsync(query.NotificationId);
    }
} 