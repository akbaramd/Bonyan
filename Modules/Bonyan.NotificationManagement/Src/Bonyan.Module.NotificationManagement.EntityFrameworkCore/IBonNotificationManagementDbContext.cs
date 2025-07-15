using Bonyan.EntityFrameworkCore;
using Bonyan.Module.NotificationManagement.Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Module.NotificationManagement.EntityFrameworkCore;

public interface IBonNotificationManagementDbContext : IEfDbContext
{
    /// <summary>
    /// DbSet for notifications
    /// </summary>
    DbSet<BonNotification> Notifications { get; set; }
}