using Bonyan.Layer.Domain.Aggregate;

namespace Bonyan.Module.NotificationManagement.Domain.Notifications;

/// <summary>
/// A very simple notification entity that targets a single user and contains only minimal information.
/// </summary>
public class BonNotification : BonFullAggregateRoot<Guid>
{
    /// <summary>
    /// The identifier of the user who should see this notification.
    /// If null, the notification is considered a broadcast to all users.
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// The title or short text of the notification.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// The main body/content of the notification.
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Optional link associated with the notification (e.g., a page the user should navigate to).
    /// </summary>
    public string? Link { get; private set; }

    /// <summary>
    /// Indicates whether the user has read this notification.
    /// </summary>
    public bool IsRead { get; private set; }

    /// <summary>
    /// The date/time the notification was read.
    /// </summary>
    public DateTime? ReadAt { get; private set; }

    /// <summary>
    /// The purpose or context of this notification (e.g., "payment_success", "order_confirmation", "welcome", etc.)
    /// </summary>
    public string? Purpose { get; private set; }

    /// <summary>
    /// Indicates whether the notification has been sent to the user.
    /// </summary>
    public bool IsSent { get; private set; }

    /// <summary>
    /// The date/time the notification was sent to the user.
    /// </summary>
    public DateTime? SentAt { get; private set; }

    /// <summary>
    /// Additional context or metadata for the notification (JSON string).
    /// </summary>
    public string? Context { get; private set; }

    protected BonNotification() { }

    public BonNotification(string? userId, string title, string message, string? link = null, string? purpose = null, string? context = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Title = title;
        Message = message;
        Link = link;
        Purpose = purpose;
        Context = context;
        IsRead = false;
        IsSent = false;
    }

    /// <summary>
    /// Marks this notification as read.
    /// </summary>
    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Marks this notification as sent.
    /// </summary>
    public void MarkAsSent()
    {
        if (!IsSent)
        {
            IsSent = true;
            SentAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Updates the message content (admin/internal use).
    /// </summary>
    public void UpdateMessage(string title, string message, string? link = null)
    {
        Title = title;
        Message = message;
        Link = link;
        UpdateModifiedDate();
    }

    /// <summary>
    /// Updates the purpose and context of the notification.
    /// </summary>
    public void UpdatePurpose(string? purpose, string? context = null)
    {
        Purpose = purpose;
        Context = context;
        UpdateModifiedDate();
    }
}