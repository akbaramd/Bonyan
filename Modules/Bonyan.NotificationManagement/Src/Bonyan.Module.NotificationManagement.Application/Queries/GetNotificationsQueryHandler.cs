using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Mediators;
using Bonyan.Module.NotificationManagement.Domain.Notifications;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;

namespace Bonyan.Module.NotificationManagement.Application.Queries;

public class GetNotificationsQueryHandler : IBonCommandHandler<GetNotificationsQuery, GetNotificationsResult>
{
    private readonly IBonNotificationReadOnlyRepository _repository;

    public GetNotificationsQueryHandler(IBonNotificationReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetNotificationsResult> HandleAsync(GetNotificationsQuery query, CancellationToken cancellationToken = default)
    {
        // Get all notifications for user
        var allNotifications = await _repository.FindAsync(n => n.UserId == query.UserId);
        
        // Apply read filter if specified
        var filteredNotifications = query.IsRead.HasValue 
            ? allNotifications.Where(n => n.IsRead == query.IsRead.Value)
            : allNotifications;
        
        // Apply pagination
        var totalCount = filteredNotifications.Count();
        var paginatedNotifications = filteredNotifications
            .OrderByDescending(n => n.CreatedAt)
            .Skip(query.Skip)
            .Take(query.Take);

        return new GetNotificationsResult(
            paginatedNotifications,
            totalCount,
            query.Skip,
            query.Take
        );
    }
} 