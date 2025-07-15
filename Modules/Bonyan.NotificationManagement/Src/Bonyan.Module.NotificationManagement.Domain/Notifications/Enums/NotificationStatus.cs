using Bonyan.Layer.Domain.Enumerations;

namespace Bonyan.Module.NotificationManagement.Domain.Notifications.Enums;

/// <summary>
/// Represents the status of a notification
/// </summary>
public class NotificationStatus : BonEnumeration
{
    public static readonly NotificationStatus Draft = new(1, nameof(Draft));
    public static readonly NotificationStatus Scheduled = new(2, nameof(Scheduled));
    public static readonly NotificationStatus Pending = new(3, nameof(Pending));
    public static readonly NotificationStatus Sending = new(4, nameof(Sending));
    public static readonly NotificationStatus Sent = new(5, nameof(Sent));
    public static readonly NotificationStatus Delivered = new(6, nameof(Delivered));
    public static readonly NotificationStatus Read = new(7, nameof(Read));
    public static readonly NotificationStatus Failed = new(8, nameof(Failed));
    public static readonly NotificationStatus Cancelled = new(9, nameof(Cancelled));
    public static readonly NotificationStatus Expired = new(10, nameof(Expired));
    
    public NotificationStatus(int id, string name) : base(id, name)
    {
    }
    
    public static IEnumerable<NotificationStatus> GetAll()
    {
        return new[]
        {
            Draft,
            Scheduled,
            Pending,
            Sending,
            Sent,
            Delivered,
            Read,
            Failed,
            Cancelled,
            Expired
        };
    }
} 