using Bonyan.Layer.Domain.ValueObjects;

namespace Bonyan.UserManagement.Domain.Users.ValueObjects;

/// <summary>
/// فرهنگ/زبان ترجیحی کاربر برای نمایش (مثلاً fa-IR، en-US).
/// </summary>
public class PreferredCulture : BonValueObject
{
    public string CultureName { get; }

    public PreferredCulture(string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
            throw new ArgumentException("Culture name cannot be null or empty.", nameof(cultureName));
        CultureName = cultureName.Trim();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CultureName;
    }

    public override string ToString() => CultureName;
}
