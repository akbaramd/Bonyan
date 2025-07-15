using Bonyan.EntityFrameworkCore;
using Bonyan.Module.NotificationManagement.Domain.Notifications;
using Bonyan.Module.NotificationManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class BonNotificationManagementDbContext : BonDbContext<BonNotificationManagementDbContext>, IBonNotificationManagementDbContext
{
    public BonNotificationManagementDbContext(DbContextOptions<BonNotificationManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<BonNotification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Notification Management entities
        modelBuilder.ConfigureNotificationManagement();
    }
}