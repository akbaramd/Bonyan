using Bonyan.Layer.Domain.Entities;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users.UserMeta;

/// <summary>
/// متای کاربر (مثل وردپرس) — فیلدهای اضافه برای هر کاربر که ماژول‌های دیگر می‌توانند اضافه یا حذف کنند.
/// </summary>
public class BonUserMeta : BonEntity
{
    public BonUserId UserId { get; private set; } = null!;
    public string MetaKey { get; private set; } = string.Empty;
    public string MetaValue { get; private set; } = string.Empty;

    protected BonUserMeta() { }

    public BonUserMeta(BonUserId userId, string metaKey, string metaValue)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        MetaKey = metaKey ?? throw new ArgumentNullException(nameof(metaKey));
        MetaValue = metaValue ?? throw new ArgumentNullException(nameof(metaValue));
    }

    public void UpdateValue(string value) => MetaValue = value ?? throw new ArgumentNullException(nameof(value));

    public override object GetKey() => new { UserId, MetaKey };
}
