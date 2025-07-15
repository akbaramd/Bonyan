using Bonyan.Layer.Domain.Enumerations;

namespace Bonyan.Module.NotificationManagement.Domain.Notifications.Enums;

/// <summary>
/// Represents the purpose or reason for the notification
/// </summary>
public class NotificationPurpose : BonEnumeration
{
    // Payment related
    public static readonly NotificationPurpose PaymentSuccess = new(1, nameof(PaymentSuccess));
    public static readonly NotificationPurpose PaymentFailed = new(2, nameof(PaymentFailed));
    public static readonly NotificationPurpose PaymentPending = new(3, nameof(PaymentPending));
    public static readonly NotificationPurpose RefundProcessed = new(4, nameof(RefundProcessed));
    
    // Order related
    public static readonly NotificationPurpose OrderConfirmed = new(10, nameof(OrderConfirmed));
    public static readonly NotificationPurpose OrderShipped = new(11, nameof(OrderShipped));
    public static readonly NotificationPurpose OrderDelivered = new(12, nameof(OrderDelivered));
    public static readonly NotificationPurpose OrderCancelled = new(13, nameof(OrderCancelled));
    
    // Account related
    public static readonly NotificationPurpose AccountCreated = new(20, nameof(AccountCreated));
    public static readonly NotificationPurpose PasswordReset = new(21, nameof(PasswordReset));
    public static readonly NotificationPurpose EmailVerified = new(22, nameof(EmailVerified));
    public static readonly NotificationPurpose ProfileUpdated = new(23, nameof(ProfileUpdated));
    
    // Security related
    public static readonly NotificationPurpose LoginAttempt = new(30, nameof(LoginAttempt));
    public static readonly NotificationPurpose SecurityAlert = new(31, nameof(SecurityAlert));
    public static readonly NotificationPurpose TwoFactorEnabled = new(32, nameof(TwoFactorEnabled));
    
    // System related
    public static readonly NotificationPurpose SystemMaintenance = new(40, nameof(SystemMaintenance));
    public static readonly NotificationPurpose FeatureUpdate = new(41, nameof(FeatureUpdate));
    public static readonly NotificationPurpose Announcement = new(42, nameof(Announcement));
    
    // Custom/Other
    public static readonly NotificationPurpose Custom = new(100, nameof(Custom));
    public static readonly NotificationPurpose Other = new(999, nameof(Other));
    
    public NotificationPurpose(int id, string name) : base(id, name)
    {
    }
    
    public static IEnumerable<NotificationPurpose> GetAll()
    {
        return new[]
        {
            // Payment related
            PaymentSuccess,
            PaymentFailed,
            PaymentPending,
            RefundProcessed,
            
            // Order related
            OrderConfirmed,
            OrderShipped,
            OrderDelivered,
            OrderCancelled,
            
            // Account related
            AccountCreated,
            PasswordReset,
            EmailVerified,
            ProfileUpdated,
            
            // Security related
            LoginAttempt,
            SecurityAlert,
            TwoFactorEnabled,
            
            // System related
            SystemMaintenance,
            FeatureUpdate,
            Announcement,
            
            // Custom/Other
            Custom,
            Other
        };
    }
} 