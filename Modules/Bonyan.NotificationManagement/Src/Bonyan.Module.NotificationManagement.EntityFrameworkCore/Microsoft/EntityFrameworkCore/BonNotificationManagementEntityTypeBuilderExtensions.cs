using Bonyan.Module.NotificationManagement.Domain.Notifications;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    public static class BonNotificationManagementEntityTypeBuilderExtensions
    {
        public static ModelBuilder ConfigureNotificationManagement(this ModelBuilder modelBuilder)
        {
            // Configure BonNotification
            modelBuilder.Entity<BonNotification>(builder =>
            {
                builder.ConfigureByConvention();
                builder.ToTable("Notifications");

                // Performance: Configure string lengths
                builder.Property(x => x.UserId)
                    .HasMaxLength(256)
                    .IsRequired(false); // Allow null for broadcast notifications

                builder.Property(x => x.Title)
                    .HasMaxLength(500)
                    .IsRequired();

                builder.Property(x => x.Message)
                    .HasMaxLength(4000)
                    .IsRequired();

                builder.Property(x => x.Link)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                builder.Property(x => x.Purpose)
                    .HasMaxLength(100)
                    .IsRequired(false);

                builder.Property(x => x.Context)
                    .HasMaxLength(2000)
                    .IsRequired(false);

                // Performance: Index for user-based queries (most common)
                builder.HasIndex(x => x.UserId)
                    .HasDatabaseName("IX_Notifications_UserId");

                // Performance: Index for read status queries
                builder.HasIndex(x => new { x.UserId, x.IsRead })
                    .HasDatabaseName("IX_Notifications_UserId_IsRead");

                // Performance: Index for sent status queries
                builder.HasIndex(x => new { x.UserId, x.IsSent })
                    .HasDatabaseName("IX_Notifications_UserId_IsSent");

                // Performance: Index for purpose queries
                builder.HasIndex(x => x.Purpose)
                    .HasDatabaseName("IX_Notifications_Purpose");

                // Performance: Index for creation date (for ordering and cleanup)
                builder.HasIndex(x => x.CreatedAt)
                    .HasDatabaseName("IX_Notifications_CreatedAt");

                // Performance: Index for read date (for analytics)
                builder.HasIndex(x => x.ReadAt)
                    .HasDatabaseName("IX_Notifications_ReadAt")
                    .HasFilter("ReadAt IS NOT NULL");

                // Performance: Index for sent date (for analytics)
                builder.HasIndex(x => x.SentAt)
                    .HasDatabaseName("IX_Notifications_SentAt")
                    .HasFilter("SentAt IS NOT NULL");

                // Performance: Composite index for user notifications with read status and date
                builder.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt })
                    .HasDatabaseName("IX_Notifications_UserId_IsRead_CreatedAt");

                // Performance: Composite index for user notifications with sent status and date
                builder.HasIndex(x => new { x.UserId, x.IsSent, x.CreatedAt })
                    .HasDatabaseName("IX_Notifications_UserId_IsSent_CreatedAt");

                // Performance: Index for unread notifications (for notification counts)
                builder.HasIndex(x => new { x.UserId, x.IsRead })
                    .HasDatabaseName("IX_Notifications_UserId_Unread")
                    .HasFilter("IsRead = 0");

                // Performance: Index for unsent notifications (for sending queue)
                builder.HasIndex(x => new { x.IsSent, x.CreatedAt })
                    .HasDatabaseName("IX_Notifications_Unsent_CreatedAt")
                    .HasFilter("IsSent = 0");

                // Performance: Index for old notifications (for cleanup operations)
                builder.HasIndex(x => new { x.CreatedAt, x.IsRead })
                    .HasDatabaseName("IX_Notifications_CreatedAt_IsRead")
                    .HasFilter("IsRead = 1");
            });

            return modelBuilder;
        }
    }
} 