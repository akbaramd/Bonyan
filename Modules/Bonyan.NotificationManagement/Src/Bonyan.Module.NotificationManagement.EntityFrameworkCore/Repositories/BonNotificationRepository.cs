using Bonyan.EntityFrameworkCore;
using Bonyan.Layer.Domain;
using Bonyan.Module.NotificationManagement.Domain.Notifications;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Module.NotificationManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreNotificationRepository : EfCoreBonRepository<BonNotification, Guid, IBonNotificationManagementDbContext>, IBonNotificationRepository
{

    protected override IQueryable<BonNotification> PrepareQuery(DbSet<BonNotification> dbSet)
    {
        return base.PrepareQuery(dbSet);
    }

    public async Task<IEnumerable<BonNotification>> GetUnreadAsync(string userId)
    {
        var query = await GetQueryableAsync();
        return await query
            .Where(x => x.UserId == userId && !x.IsRead)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BonNotification>> GetUnsentAsync()
    {
        var query = await GetQueryableAsync();
        return await query
            .Where(x => !x.IsSent)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BonNotification>> GetByPurposeAsync(string purpose)
    {
        var query = await GetQueryableAsync();
        return await query
            .Where(x => x.Purpose == purpose)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BonNotification>> GetByPurposeAsync(string userId, string purpose)
    {
        var query = await GetQueryableAsync();
        return await query
            .Where(x => x.UserId == userId && x.Purpose == purpose)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var unreadNotifications = await (await GetQueryableAsync())
            .Where(x => x.UserId == userId && !x.IsRead)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }
    }

    public async Task MarkAsSentAsync(Guid notificationId)
    {
        var notification = await GetByIdAsync(notificationId);
        if (notification != null)
        {
            notification.MarkAsSent();
        }
    }

    public async Task<int> CountUnsentAsync()
    {
        var query = await GetQueryableAsync();
        return await query
            .CountAsync(x => !x.IsSent);
    }
}

public class BonEfCoreNotificationReadOnlyRepository : EfCoreReadonlyRepository<BonNotification, Guid, IBonNotificationManagementDbContext>, IBonNotificationReadOnlyRepository
{

    protected override IQueryable<BonNotification> PrepareQuery(DbSet<BonNotification> dbSet)
    {
        return base.PrepareQuery(dbSet);
    }

    public async Task<IEnumerable<BonNotification>> GetUnreadAsync(string userId)
    {
        var query = await GetQueryableAsync();
        return await query
            .Where(x => x.UserId == userId && !x.IsRead)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BonNotification>> GetUnsentAsync()
    {
        var query = await GetQueryableAsync();
        return await query
            .Where(x => !x.IsSent)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BonNotification>> GetByPurposeAsync(string purpose)
    {
        var query = await GetQueryableAsync();
        return await query
            .Where(x => x.Purpose == purpose)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<BonNotification>> GetByPurposeAsync(string userId, string purpose)
    {
        var query = await GetQueryableAsync();
        return await query
            .Where(x => x.UserId == userId && x.Purpose == purpose)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> CountUnreadAsync(string userId)
    {  
        var query = await GetQueryableAsync();
        return await query
            .CountAsync(x => x.UserId == userId && !x.IsRead);
    }

    public async Task<int> CountUnsentAsync()
    {
        var query = await GetQueryableAsync();
        return await query
            .CountAsync(x => !x.IsSent);
    }
} 