# Bonyan.UserManagement.Domain

دامنه **کاربر** به‌صورت خالص — بدون رمز عبور، توکن، قفل حساب یا هر مفهوم احراز هویت.

این ماژول فقط مدل غنی کاربر (پروفایل، اطلاعات تماس، وضعیت) را تعریف می‌کند. احراز هویت و identity در ماژول **Bonyan.IdentityManagement** است که از این دامنه گسترش می‌گیرد.

## موجودیت‌ها و ارزش‌شیءها

| نوع | توضیح |
|-----|--------|
| **BonUser** | aggregate root کاربر: نام کاربری، رایانامه، تلفن، وضعیت، پروفایل، جنسیت، فرهنگ، منطقه زمانی، آواتار |
| **UserProfile** | ارزش‌شیء: نام، نام خانوادگی، نام نمایشی، تاریخ تولد، کد ملی |
| **BonUserEmail** | رایانامه با وضعیت تأیید |
| **BonUserPhoneNumber** | شماره تلفن با وضعیت تأیید |
| **PreferredCulture** | فرهنگ/زبان ترجیحی (مثلاً fa-IR) |
| **Gender** | جنسیت (اختیاری؛ برای پروفایل) |
| **UserStatus** | وضعیت کاربر: فعال، تعلیق، غیرفعال، قفل، و غیره |

## رفتارهای دامنه (بدون احراز هویت)

- `UpdateProfile(userName, email, phoneNumber)` — به‌روزرسانی اطلاعات تماس
- `UpdatePersonalProfile(UserProfile)` — به‌روزرسانی پروفایل شخصی
- `SetGender`, `SetPreferredCulture`, `SetTimeZone`, `SetAvatarUrl`
- `SetEmail`, `SetPhoneNumber`
- `VerifyEmail`, `VerifyPhoneNumber` — فقط علامت‌گذاری تأیید
- `ChangeStatus(UserStatus)` — تغییر وضعیت کاربر

## رویدادهای دامنه

- `BonUserCreatedDomainEvent`
- `BonUserProfileUpdatedDomainEvent`
- `BonUserEmailVerifiedDomainEvent`
- `BonUserPhoneNumberVerifiedDomainEvent`
- `BonUserStatusChangedDomainEvent`

## ارتباط با Identity

ماژول **IdentityManagement** از `BonUser` ارث می‌برد و نوع `BonIdentityUser<TUser, TRole>` را تعریف می‌کند که **رمز عبور، توکن، نقش و ادعا** را اضافه می‌کند. پروفایل کاربر از همین دامنه (UserManagement) استفاده می‌کند.
