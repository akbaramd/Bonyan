using Bonyan.Layer.Domain.Enumerations;

namespace Bonyan.Module.NotificationManagement.Domain.Notifications.Enums;

/// <summary>
/// Represents the type of notification (global categories)
/// </summary>
public class NotificationType : BonEnumeration
{
    public static readonly NotificationType Email = new(1, nameof(Email));
    public static readonly NotificationType Sms = new(2, nameof(Sms));
    public static readonly NotificationType Push = new(3, nameof(Push));
    public static readonly NotificationType InApp = new(4, nameof(InApp));
    public static readonly NotificationType Webhook = new(5, nameof(Webhook));
    public static readonly NotificationType Chat = new(6, nameof(Chat)); // Generic chat channel (Slack, Discord, Teams, etc.)
    
    public NotificationType(int id, string name) : base(id, name)
    {
    }
    
    public static IEnumerable<NotificationType> GetAll()
    {
        return new[]
        {
            Email,
            Sms,
            Push,
            InApp,
            Webhook,
            Chat
        };
    }
} 