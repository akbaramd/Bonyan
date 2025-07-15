using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;

public interface IBonNotificationRepository : IBonRepository<BonNotification,Guid>
{
    Task<IEnumerable<BonNotification>> GetUnreadAsync(string userId);
    Task<IEnumerable<BonNotification>> GetUnsentAsync();
    Task<IEnumerable<BonNotification>> GetByPurposeAsync(string purpose);
    Task<IEnumerable<BonNotification>> GetByPurposeAsync(string userId, string purpose);
    Task MarkAllAsReadAsync(string userId);
    Task MarkAsSentAsync(Guid notificationId);
    Task<int> CountUnsentAsync();
}

public interface IBonNotificationReadOnlyRepository : IBonReadOnlyRepository<BonNotification,Guid>
{
    Task<IEnumerable<BonNotification>> GetUnreadAsync(string userId);
    Task<IEnumerable<BonNotification>> GetUnsentAsync();
    Task<IEnumerable<BonNotification>> GetByPurposeAsync(string purpose);
    Task<IEnumerable<BonNotification>> GetByPurposeAsync(string userId, string purpose);
    Task<int> CountUnreadAsync(string userId);
    Task<int> CountUnsentAsync();
}