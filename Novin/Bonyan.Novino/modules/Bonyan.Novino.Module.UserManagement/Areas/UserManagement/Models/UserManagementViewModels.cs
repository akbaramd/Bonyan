using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Bonyan.Novino.Module.UserManagement.Models
{
    /// <summary>
    /// Simplified filter model for user management
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

        // Helper properties
        public bool HasFilters => !string.IsNullOrWhiteSpace(SearchTerm) || !string.IsNullOrWhiteSpace(Status);
    }

    /// <summary>
    /// View model for user list item
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
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        public string FullName => $"{Name} {SurName}".Trim();
        public string Status => IsActive ? "فعال" : "غیرفعال";
        public string StatusBadgeClass => IsActive ? "bg-success" : "bg-danger";
        public string LockStatus => IsLocked ? "قفل شده" : "باز";
        public string LockBadgeClass => IsLocked ? "bg-warning" : "bg-success";
    }

    /// <summary>
    /// View model for user list index page
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
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanExport { get; set; }
        public bool CanBulkDelete { get; set; }

        // Available filter options
        public List<string> AvailableRoles { get; set; } = new();
    }

    /// <summary>
    /// View model for user details page
    /// </summary>
    public class UserDetailsViewModel
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
        public DateTime? LastLoginAt { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        // Permission flags
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanChangePassword { get; set; }
        public bool CanResetPassword { get; set; }
        public bool CanLock { get; set; }
        public bool CanUnlock { get; set; }
        public bool CanActivate { get; set; }
        public bool CanDeactivate { get; set; }

        public string FullName => $"{Name} {SurName}".Trim();
        public string Status => IsActive ? "فعال" : "غیرفعال";
        public string StatusBadgeClass => IsActive ? "bg-success" : "bg-danger";
        public string LockStatus => IsLocked ? "قفل شده" : "باز";
        public string LockBadgeClass => IsLocked ? "bg-warning" : "bg-success";
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
        public string Roles { get; set; } = string.Empty;
        public string IsActive { get; set; } = string.Empty;
        public string IsLocked { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string LastLoginAt { get; set; } = string.Empty;
        public string EmailConfirmed { get; set; } = string.Empty;
        public string PhoneNumberConfirmed { get; set; } = string.Empty;
    }

    /// <summary>
    /// View model for creating/editing user
    /// </summary>
    public class UserCreateEditViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام الزامی است")]
        [Display(Name = "نام")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام خانوادگی الزامی است")]
        [Display(Name = "نام خانوادگی")]
        public string SurName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "فرمت شماره تلفن صحیح نیست")]
        [Display(Name = "شماره تلفن")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "تأیید ایمیل")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "تأیید شماره تلفن")]
        public bool PhoneNumberConfirmed { get; set; }

        [Display(Name = "نقش‌ها")]
        public List<string> SelectedRoles { get; set; } = new();

        public List<RoleViewModel> AvailableRoles { get; set; } = new();
    }

    public class RoleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
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
        [Display(Name = "تأیید رمز عبور")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "اجبار تغییر رمز عبور در ورود بعدی")]
        public bool ForceChangePassword { get; set; }
    }
} 