using Bonyan.Layer.Domain.ValueObjects;

namespace Bonyan.UserManagement.Domain.Users.ValueObjects;

/// <summary>
/// پروفایل شخصی کاربر (نام، نام خانوادگی، تاریخ تولد، کد ملی). فاقد هرگونه مفهوم احراز هویت.
/// User profile value object: display name, first/last name, date of birth, national code. No auth concepts.
/// </summary>
public class UserProfile : BonValueObject
{
    /// <summary>نام</summary>
    public string FirstName { get; }

    /// <summary>نام خانوادگی</summary>
    public string LastName { get; }

    /// <summary>نام نمایشی (اگر خالی باشد برابر نام کامل است)</summary>
    public string DisplayName { get; }

    /// <summary>تاریخ تولد (اختیاری)</summary>
    public DateTime? DateOfBirth { get; }

    /// <summary>کد ملی (اختیاری)</summary>
    public string? NationalCode { get; }

    public UserProfile(string firstName, string lastName, string? displayName = null, DateTime? dateOfBirth = null, string? nationalCode = null)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? $"{FirstName} {LastName}".Trim() : displayName;
        DateOfBirth = dateOfBirth;
        NationalCode = nationalCode;
    }

    /// <summary>
    /// نام کامل: نام + نام خانوادگی
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        yield return DisplayName;
        yield return DateOfBirth;
        yield return NationalCode;
    }
}
