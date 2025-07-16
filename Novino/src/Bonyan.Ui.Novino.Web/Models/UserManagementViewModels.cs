using System.ComponentModel.DataAnnotations;

namespace Bonyan.Novino.Web.Models
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

        // Helper properties
        public bool HasFilters => !string.IsNullOrWhiteSpace(SearchTerm);

        public string GetQueryString()
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                queryParams.Add($"searchTerm={Uri.EscapeDataString(SearchTerm)}");

            queryParams.Add($"page={Page}");
            queryParams.Add($"pageSize={PageSize}");
            queryParams.Add($"sortBy={SortBy}");
            queryParams.Add($"sortOrder={SortOrder}");

            return string.Join("&", queryParams);
        }

        public static UserFilterModel FromQueryString(IQueryCollection query)
        {
            var model = new UserFilterModel();

            if (query.ContainsKey("searchTerm"))
                model.SearchTerm = query["searchTerm"].ToString();

            if (query.ContainsKey("page") && int.TryParse(query["page"], out var page))
                model.Page = page;

            if (query.ContainsKey("pageSize") && int.TryParse(query["pageSize"], out var pageSize))
                model.PageSize = pageSize;

            if (query.ContainsKey("sortBy"))
                model.SortBy = query["sortBy"].ToString();

            if (query.ContainsKey("sortOrder"))
                model.SortOrder = query["sortOrder"].ToString();

            return model;
        }
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
        [StringLength(100, MinimumLength = 3, ErrorMessage = "نام کاربری باید بین ۳ تا ۱۰۰ کاراکتر باشد")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام الزامی است")]
        [StringLength(50, ErrorMessage = "نام نمی‌تواند بیش از ۵۰ کاراکتر باشد")]
        [Display(Name = "نام")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام خانوادگی الزامی است")]
        [StringLength(50, ErrorMessage = "نام خانوادگی نمی‌تواند بیش از ۵۰ کاراکتر باشد")]
        [Display(Name = "نام خانوادگی")]
        public string SurName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "فرمت شماره تلفن صحیح نیست")]
        [Display(Name = "شماره تلفن")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "تأیید ایمیل")]
        public bool EmailConfirmed { get; set; } = false;

        [Display(Name = "تأیید شماره تلفن")]
        public bool PhoneNumberConfirmed { get; set; } = false;

        [Display(Name = "نقش‌ها")]
        public List<string> SelectedRoles { get; set; } = new();

        public List<RoleViewModel> AvailableRoles { get; set; } = new();
    }

    /// <summary>
    /// View model for role selection
    /// </summary>
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
        [StringLength(128, MinimumLength = 8, ErrorMessage = "رمز عبور باید حداقل ۸ کاراکتر باشد")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", 
            ErrorMessage = "رمز عبور باید شامل حروف کوچک، بزرگ و اعداد باشد")]
        [Display(Name = "رمز عبور جدید")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "تأیید رمز عبور الزامی است")]
        [Compare("NewPassword", ErrorMessage = "رمز عبور و تأیید آن مطابقت ندارند")]
        [Display(Name = "تأیید رمز عبور")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "اجبار تغییر رمز عبور در ورود بعدی")]
        public bool RequirePasswordChange { get; set; } = true;
    }
} 