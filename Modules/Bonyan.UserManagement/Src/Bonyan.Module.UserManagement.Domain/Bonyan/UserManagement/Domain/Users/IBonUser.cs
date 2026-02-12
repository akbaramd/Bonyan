using Bonyan.Layer.Domain.Aggregate.Abstractions;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.UserManagement.Domain.Users;

/// <summary>
/// قرارداد aggregate کاربر — تنها پروفایل و اطلاعات تماس. بدون احراز هویت.
/// User aggregate contract: profile and contact only. No authentication.
/// </summary>
public interface IBonUser : IBonAggregateRoot<BonUserId>
{
    string UserName { get; }
    BonUserEmail? Email { get; }
    BonUserPhoneNumber? PhoneNumber { get; }
    UserStatus Status { get; }
    Guid Version { get; }

    /// <summary>پروفایل شخصی (نام، نام خانوادگی، کد ملی، تاریخ تولد)</summary>
    UserProfile? Profile { get; }

    /// <summary>جنسیت (اختیاری)</summary>
    Gender? Gender { get; }

    /// <summary>فرهنگ/زبان ترجیحی</summary>
    PreferredCulture? PreferredCulture { get; }

    /// <summary>منطقه زمانی</summary>
    string? TimeZoneId { get; }

    /// <summary>آدرس تصویر پروفایل</summary>
    string? AvatarUrl { get; }

    void UpdateProfile(string userName, BonUserEmail? email, BonUserPhoneNumber? phoneNumber);
    void UpdatePersonalProfile(UserProfile profile);
    void SetGender(Gender? gender);
    void SetPreferredCulture(PreferredCulture? culture);
    void SetTimeZone(string? timeZoneId);
    void SetAvatarUrl(string? avatarUrl);
    void SetPhoneNumber(BonUserPhoneNumber? phoneNumber);
    void SetEmail(BonUserEmail? email);
    void VerifyEmail();
    void VerifyPhoneNumber();
    void ChangeStatus(UserStatus newStatus);
}
