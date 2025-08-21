using System.ComponentModel.DataAnnotations;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Utils;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Models
{
    /// <summary>
    /// Enhanced filter model for user management with advanced filtering options
    /// </summary>
    public class UserFilterModel
    {
        // Search
        [Display(Name = "جستجو")]
        public string? SearchTerm { get; set; }

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sorting
        public string SortBy { get; set; } = "UserName";
        public string SortOrder { get; set; } = "asc";

        // Status filter
        public string? Status { get; set; }

        // Advanced filters
        [Display(Name = "نقش")]
        public string? RoleFilter { get; set; }

        [Display(Name = "تأیید ایمیل")]
        public bool? EmailConfirmed { get; set; }

        [Display(Name = "تأیید شماره تلفن")]
        public bool? PhoneConfirmed { get; set; }

        [Display(Name = "تاریخ ایجاد از")]
        public DateTime? CreatedFrom { get; set; }

        [Display(Name = "تاریخ ایجاد تا")]
        public DateTime? CreatedTo { get; set; }

        // Helper properties
        public bool HasFilters => !string.IsNullOrWhiteSpace(SearchTerm) || 
                                 !string.IsNullOrWhiteSpace(Status) || 
                                 !string.IsNullOrWhiteSpace(RoleFilter) ||
                                 EmailConfirmed.HasValue ||
                                 PhoneConfirmed.HasValue ||
                                 CreatedFrom.HasValue ||
                                 CreatedTo.HasValue;
    }

    /// <summary>
    /// Enhanced view model for user list item with more details
    /// </summary>
    public class UserListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SurName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? AccountLockedUntil { get; set; }

        public string FullName => $"{Name} {SurName}".Trim();
        public string Status => IsActive ? "فعال" : "غیرفعال";
        public string StatusBadgeClass => IsActive ? "bg-success" : "bg-danger";
        public string LockStatus => IsLocked ? "قفل شده" : "باز";
        public string LockBadgeClass => IsLocked ? "bg-warning" : "bg-success";
        public string EmailStatus => EmailConfirmed ? "تأیید شده" : "تأیید نشده";
        public string EmailStatusClass => EmailConfirmed ? "text-success" : "text-warning";
        public string PhoneStatus => PhoneNumberConfirmed ? "تأیید شده" : "تأیید نشده";
        public string PhoneStatusClass => PhoneNumberConfirmed ? "text-success" : "text-warning";
        public string CreatedAtFormatted => CreatedAt.ToString("yyyy/MM/dd HH:mm");
        public string LastLoginFormatted => LastLoginAt?.ToString("yyyy/MM/dd HH:mm") ?? "هرگز";
    }

    /// <summary>
    /// Enhanced view model for user list index page with advanced features
    /// </summary>
    public class UserListIndexViewModel
    {
        public List<UserListViewModel> Users { get; set; } = new();
        public UserFilterModel Filter { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        // Permission flags
        public bool CanCreate { get; set; }
        public bool CanDetails { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanExport { get; set; }
        public bool CanBulkDelete { get; set; }
        public bool CanBulkActivate { get; set; }
        public bool CanBulkDeactivate { get; set; }

        // Available filter options
        public List<string> AvailableRoles { get; set; } = new();
        public List<string> AvailableStatuses { get; set; } = new() { "فعال", "غیرفعال", "قفل شده" };
    }

    /// <summary>
    /// Comprehensive view model for user details page with tabs
    /// </summary>
    public class UserDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SurName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? NationalCode { get; set; }
        public string? DateOfBirth { get; set; }
        public List<UserClaimViewModel> Claims { get; set; } = new();
        public List<UserTokenViewModel> Tokens { get; set; } = new();
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? AccountLockedUntil { get; set; }
        public DateTime? BannedUntil { get; set; }
        public bool CanBeDeleted { get; set; }

        // Permission flags
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanChangePassword { get; set; }
        public bool CanResetPassword { get; set; }
        public bool CanLock { get; set; }
        public bool CanUnlock { get; set; }
        public bool CanActivate { get; set; }
        public bool CanDeactivate { get; set; }
        public bool CanManageRoles { get; set; }
        public bool CanManageClaims { get; set; }

        public string FullName => $"{Name} {SurName}".Trim();
        public string Status => IsActive ? "فعال" : "غیرفعال";
        public string StatusBadgeClass => IsActive ? "bg-success" : "bg-danger";
        public string LockStatus => IsLocked ? "قفل شده" : "باز";
        public string LockBadgeClass => IsLocked ? "bg-warning" : "bg-success";
        public string EmailStatus => EmailConfirmed ? "تأیید شده" : "تأیید نشده";
        public string EmailStatusClass => EmailConfirmed ? "text-success" : "text-warning";
        public string PhoneStatus => PhoneNumberConfirmed ? "تأیید شده" : "تأیید نشده";
        public string PhoneStatusClass => PhoneNumberConfirmed ? "text-success" : "text-warning";
        public string CreatedAtFormatted => CreatedAt.ToString("yyyy/MM/dd HH:mm");
        public string LastLoginFormatted => LastLoginAt?.ToString("yyyy/MM/dd HH:mm") ?? "هرگز";
        public string DateOfBirthFormatted => !string.IsNullOrWhiteSpace(DateOfBirth) ? 
            DateOfBirth : 
            "تعیین نشده";
        public int Age => PersianDateConverter.GetAgeFromPersianDate(DateOfBirth);
    }

    /// <summary>
    /// View model for user roles in details page
    /// </summary>
    public class UserRoleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public bool CanBeRemoved { get; set; }
    }

    /// <summary>
    /// View model for user claims in details page
    /// </summary>
    public class UserClaimViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string ClaimType { get; set; } = string.Empty;
        public string ClaimValue { get; set; } = string.Empty;
        public string? Issuer { get; set; }
    }

    /// <summary>
    /// View model for user tokens in details page
    /// </summary>
    public class UserTokenViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? Expiration { get; set; }
        public bool IsExpired => Expiration.HasValue && DateTime.UtcNow > Expiration.Value;
    }

    /// <summary>
    /// Enhanced view model for creating/editing user with validation
    /// </summary>
    public class UserCreateEditViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [StringLength(256, ErrorMessage = "نام کاربری نمی‌تواند بیشتر از {1} کاراکتر باشد")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست")]
        [StringLength(256, ErrorMessage = "ایمیل نمی‌تواند بیشتر از {1} کاراکتر باشد")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام الزامی است")]
        [StringLength(100, ErrorMessage = "نام نمی‌تواند بیشتر از {1} کاراکتر باشد")]
        [Display(Name = "نام")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام خانوادگی الزامی است")]
        [StringLength(100, ErrorMessage = "نام خانوادگی نمی‌تواند بیشتر از {1} کاراکتر باشد")]
        [Display(Name = "نام خانوادگی")]
        public string SurName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "فرمت شماره تلفن صحیح نیست")]
        [StringLength(20, ErrorMessage = "شماره تلفن نمی‌تواند بیشتر از {1} کاراکتر باشد")]
        [Display(Name = "شماره تلفن")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "کد ملی الزامی است")]
        [StringLength(50, ErrorMessage = "کد ملی نمی‌تواند بیشتر از {1} کاراکتر باشد")]
        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }

        [Display(Name = "تاریخ تولد")]
        public string? DateOfBirth { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "تأیید ایمیل")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "تأیید شماره تلفن")]
        public bool PhoneNumberConfirmed { get; set; }

        [Display(Name = "نقش‌ها")]
        public List<string> SelectedRoles { get; set; } = new();

        public List<RoleViewModel> AvailableRoles { get; set; } = new();

        // Password fields (only for create)
        [StringLength(100, ErrorMessage = "رمز عبور باید حداقل {2} کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "رمز عبور و تأیید آن مطابقت ندارند")]
        [DataType(DataType.Password)]
        [Display(Name = "تأیید رمز عبور")]
        public string? ConfirmPassword { get; set; }

        public bool IsCreate => string.IsNullOrEmpty(Id);
        public bool ForceChangePassword { get; set; }
    }

    /// <summary>
    /// Enhanced role view model
    /// </summary>
    public class RoleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public bool CanBeDeleted { get; set; }
    }

    /// <summary>
    /// View model for changing user password
    /// </summary>
    public class ChangeUserPasswordViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز عبور جدید الزامی است")]
        [StringLength(100, ErrorMessage = "رمز عبور باید حداقل {2} کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور جدید")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "تأیید رمز عبور الزامی است")]
        [Compare("NewPassword", ErrorMessage = "رمز عبور و تأیید آن مطابقت ندارند")]
        [DataType(DataType.Password)]
        [Display(Name = "تأیید رمز عبور جدید")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "اجبار تغییر رمز عبور در ورود بعدی")]
        public bool ForceChangePassword { get; set; } = true;
    }

    /// <summary>
    /// View model for bulk operations
    /// </summary>
    public class BulkOperationViewModel
    {
        [Required(ErrorMessage = "انتخاب کاربران الزامی است")]
        [Display(Name = "کاربران انتخاب شده")]
        public List<string> SelectedUserIds { get; set; } = new();

        [Required(ErrorMessage = "نوع عملیات الزامی است")]
        [Display(Name = "نوع عملیات")]
        public string Operation { get; set; } = string.Empty;

        public string? Reason { get; set; }
    }

    /// <summary>
    /// View model for user export
    /// </summary>
    public class UserExportViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SurName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? NationalCode { get; set; }
        public string DateOfBirth { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
        public string IsActive { get; set; } = string.Empty;
        public string IsLocked { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string LastLoginAt { get; set; } = string.Empty;
        public string EmailConfirmed { get; set; } = string.Empty;
        public string PhoneNumberConfirmed { get; set; } = string.Empty;
        public string FailedLoginAttempts { get; set; } = string.Empty;
    }

    /// <summary>
    /// View model for user delete confirmation page
    /// </summary>
    public class UserDeleteViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SurName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool CanBeDeleted { get; set; }

        public string FullName => $"{Name} {SurName}".Trim();
        public string Status => IsActive ? "فعال" : "غیرفعال";
        public string StatusBadgeClass => IsActive ? "bg-success" : "bg-danger";
        public string LockStatus => IsLocked ? "قفل شده" : "باز";
        public string LockBadgeClass => IsLocked ? "bg-warning" : "bg-success";
        public string CreatedAtFormatted => CreatedAt.ToString("yyyy/MM/dd HH:mm");
    }
} 