using Bonyan.Layer.Domain.Enumerations;

namespace Bonyan.Module.NotificationManagement.Domain.Notifications.Enums;

/// <summary>
/// Represents the priority level of a notification
/// </summary>
public class NotificationPriority : BonEnumeration
{
    public static readonly NotificationPriority Low = new(1, nameof(Low));
    public static readonly NotificationPriority Normal = new(2, nameof(Normal));
    public static readonly NotificationPriority High = new(3, nameof(High));
    public static readonly NotificationPriority Urgent = new(4, nameof(Urgent));
    public static readonly NotificationPriority Critical = new(5, nameof(Critical));
    
    public NotificationPriority(int id, string name) : base(id, name)
    {
    }
    
    public static IEnumerable<NotificationPriority> GetAll()
    {
        return new[]
        {
            Low,
            Normal,
            High,
            Urgent,
            Critical
        };
    }
} 