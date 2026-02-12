using System.ComponentModel.DataAnnotations;
using Bonyan.Layer.Domain.Aggregate;
using Bonyan.UserManagement.Domain.Users.DomainEvents;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.UserManagement.Domain.Users;

/// <summary>
/// موجودیت کاربر در دامنه — صرفاً پروفایل و اطلاعات تماس. بدون رمز عبور، توکن یا هر مفهوم احراز هویت.
/// User aggregate: profile, contact info, status. No password, tokens, or any authentication concept.
/// </summary>
public class BonUser : BonFullAggregateRoot<BonUserId>, IBonUser
{
    /// <summary>نام کاربری یکتا (شناسه نمایشی برای کاربر)</summary>
    public string UserName { get; private set; } = string.Empty;

    /// <summary>رایانامه (اختیاری)</summary>
    public BonUserEmail? Email { get; private set; }

    /// <summary>شماره تلفن (اختیاری)</summary>
    public BonUserPhoneNumber? PhoneNumber { get; private set; }

    /// <summary>وضعیت کاربر در سیستم (فعال، تعلیق، غیرفعال و ...)</summary>
    public UserStatus Status { get; private set; }

    /// <summary>پروفایل شخصی (نام، نام خانوادگی، کد ملی، تاریخ تولد)</summary>
    public UserProfile? Profile { get; private set; }

    /// <summary>جنسیت (اختیاری)</summary>
    public Gender? Gender { get; private set; }

    /// <summary>فرهنگ/زبان ترجیحی (مثلاً fa-IR)</summary>
    public PreferredCulture? PreferredCulture { get; private set; }

    /// <summary>منطقه زمانی (مثلاً Asia/Tehran)</summary>
    public string? TimeZoneId { get; private set; }

    /// <summary>آدرس تصویر پروفایل (اختیاری)</summary>
    public string? AvatarUrl { get; private set; }

    [ConcurrencyCheck]
    public Guid Version { get; private set; }

    protected BonUser() { }

    /// <summary>
    /// ایجاد کاربر با حداقل اطلاعات (شناسه و نام کاربری).
    /// </summary>
    public BonUser(BonUserId id, string userName)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        Status = UserStatus.Active;
        AddDomainEvent(new BonUserCreatedDomainEvent(this));
    }

    /// <summary>
    /// ایجاد کاربر با اطلاعات تماس (بدون هیچ مفهوم احراز هویت).
    /// </summary>
    public BonUser(BonUserId id, string userName, BonUserEmail? email, BonUserPhoneNumber? phoneNumber)
        : this(id, userName)
    {
        Email = email;
        PhoneNumber = phoneNumber;
        AddDomainEvent(new BonUserProfileUpdatedDomainEvent(this));
    }

    /// <summary>
    /// ایجاد کاربر با پروفایل و اطلاعات تماس.
    /// </summary>
    public BonUser(BonUserId id, string userName, UserProfile profile, BonUserEmail? email = null, BonUserPhoneNumber? phoneNumber = null)
        : this(id, userName, email, phoneNumber)
    {
        Profile = profile ?? throw new ArgumentNullException(nameof(profile));
    }

    // ——— رفتارهای پروفایل و اطلاعات تماس ———

    /// <summary>
    /// به‌روزرسانی نام کاربری، رایانامه و شماره تلفن.
    /// </summary>
    public void UpdateProfile(string userName, BonUserEmail? email, BonUserPhoneNumber? phoneNumber)
    {
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        Email = email;
        PhoneNumber = phoneNumber;
        UpdateModifiedDate();
        AddDomainEvent(new BonUserProfileUpdatedDomainEvent(this));
    }

    /// <summary>
    /// به‌روزرسانی پروفایل شخصی (نام، نام خانوادگی، کد ملی، تاریخ تولد).
    /// </summary>
    public void UpdatePersonalProfile(UserProfile profile)
    {
        Profile = profile ?? throw new ArgumentNullException(nameof(profile));
        UpdateModifiedDate();
        AddDomainEvent(new BonUserProfileUpdatedDomainEvent(this));
    }

    /// <summary>
    /// تنظیم جنسیت (اختیاری).
    /// </summary>
    public void SetGender(Gender? gender)
    {
        Gender = gender;
        UpdateModifiedDate();
    }

    /// <summary>
    /// تنظیم فرهنگ/زبان ترجیحی.
    /// </summary>
    public void SetPreferredCulture(PreferredCulture? culture)
    {
        PreferredCulture = culture;
        UpdateModifiedDate();
    }

    /// <summary>
    /// تنظیم منطقه زمانی (مثلاً Asia/Tehran).
    /// </summary>
    public void SetTimeZone(string? timeZoneId)
    {
        TimeZoneId = timeZoneId;
        UpdateModifiedDate();
    }

    /// <summary>
    /// تنظیم آدرس تصویر پروفایل.
    /// </summary>
    public void SetAvatarUrl(string? avatarUrl)
    {
        AvatarUrl = avatarUrl;
        UpdateModifiedDate();
    }

    public void SetPhoneNumber(BonUserPhoneNumber? bonUserPhoneNumber)
    {
        PhoneNumber = bonUserPhoneNumber;
        UpdateModifiedDate();
        AddDomainEvent(new BonUserProfileUpdatedDomainEvent(this));
    }

    public void SetEmail(BonUserEmail? bonUserEmail)
    {
        Email = bonUserEmail;
        UpdateModifiedDate();
        AddDomainEvent(new BonUserProfileUpdatedDomainEvent(this));
    }

    /// <summary>
    /// تأیید رایانامه (علامت‌گذاری به‌عنوان تأییدشده).
    /// </summary>
    public void VerifyEmail()
    {
        if (Email != null && !Email.IsVerified)
        {
            Email.Verify();
            AddDomainEvent(new BonUserEmailVerifiedDomainEvent(this));
        }
    }

    /// <summary>
    /// تأیید شماره تلفن.
    /// </summary>
    public void VerifyPhoneNumber()
    {
        if (PhoneNumber != null && !PhoneNumber.IsVerified)
        {
            PhoneNumber.Verify();
            AddDomainEvent(new BonUserPhoneNumberVerifiedDomainEvent(this));
        }
    }

    /// <summary>
    /// تغییر وضعیت کاربر (فعال، تعلیق، غیرفعال و ...).
    /// </summary>
    public void ChangeStatus(UserStatus newStatus)
    {
        if (Status != newStatus)
        {
            Status = newStatus;
            UpdateModifiedDate();
            AddDomainEvent(new BonUserStatusChangedDomainEvent(this, newStatus));
        }
    }
}
