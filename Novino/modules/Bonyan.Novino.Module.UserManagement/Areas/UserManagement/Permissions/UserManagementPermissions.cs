using Bonyan.IdentityManagement.Permissions;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Permissions;

/// <summary>
/// Permission constants for User Management module
/// </summary>
public static class UserManagementPermissions
{
    public const string GroupName = "UserManagement";

    public static class Users
    {
        public const string Default = GroupName + ".Users";
        public const string View = Default + ".View";
        public const string Details = Default + ".Details";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Export = Default + ".Export";
        public const string BulkDelete = Default + ".BulkDelete";
        public const string BulkActivate = Default + ".BulkActivate";
        public const string BulkDeactivate = Default + ".BulkDeactivate";
        public const string ChangePassword = Default + ".ChangePassword";
        public const string ResetPassword = Default + ".ResetPassword";
        public const string Lock = Default + ".Lock";
        public const string Unlock = Default + ".Unlock";
        public const string Activate = Default + ".Activate";
        public const string Deactivate = Default + ".Deactivate";
        public const string ManageRoles = Default + ".ManageRoles";
        public const string ManageClaims = Default + ".ManageClaims";
    }
}

/// <summary>
/// Permission definition provider for User Management module
/// </summary>
public class UserManagementPermissionProvider : IBonPermissionDefinitionProvider
{
    public void Define(IPermissionDefinitionContext context)
    {
        var userManagementGroup = context.AddGroup(
            UserManagementPermissions.GroupName,
            "مدیریت کاربران", // User Management
            "دسترسی‌های لازم برای مدیریت کاربران" // Permissions required for user management
        );

        DefineUserPermissions(userManagementGroup);
    }

    private static void DefineUserPermissions(PermissionGroupDefinition group)
    {
        var users = group.AddPermission(
            UserManagementPermissions.Users.Default,
            "مدیریت کاربران", // User Management
            "دسترسی عمومی به بخش مدیریت کاربران" // General access to user management section
        );

        users.AddChild(
            UserManagementPermissions.Users.View,
            "مشاهده کاربران", // View Users
            "مشاهده لیست کاربران و اطلاعات آن‌ها" // View user list and their information
        );

        users.AddChild(
            UserManagementPermissions.Users.Details,
            "جزئیات کاربر", // User Details
            "مشاهده جزئیات کامل کاربر و اطلاعات مربوطه" // View complete user details and related information
        );

        users.AddChild(
            UserManagementPermissions.Users.Create,
            "ایجاد کاربر", // Create User
            "ایجاد کاربر جدید در سیستم" // Create new user in the system
        );

        users.AddChild(
            UserManagementPermissions.Users.Edit,
            "ویرایش کاربر", // Edit User
            "ویرایش اطلاعات کاربران موجود" // Edit existing user information
        );

        users.AddChild(
            UserManagementPermissions.Users.Delete,
            "حذف کاربر", // Delete User
            "حذف کاربران از سیستم" // Delete users from the system
        );

        users.AddChild(
            UserManagementPermissions.Users.Export,
            "خروجی کاربران", // Export Users
            "خروجی گرفتن از لیست کاربران" // Export user list
        );

        users.AddChild(
            UserManagementPermissions.Users.BulkDelete,
            "حذف گروهی کاربران", // Bulk Delete Users
            "حذف همزمان چندین کاربر" // Delete multiple users at once
        );

        users.AddChild(
            UserManagementPermissions.Users.BulkActivate,
            "فعال‌سازی گروهی کاربران", // Bulk Activate Users
            "فعال‌سازی همزمان چندین کاربر" // Activate multiple users at once
        );

        users.AddChild(
            UserManagementPermissions.Users.BulkDeactivate,
            "غیرفعال‌سازی گروهی کاربران", // Bulk Deactivate Users
            "غیرفعال‌سازی همزمان چندین کاربر" // Deactivate multiple users at once
        );

        users.AddChild(
            UserManagementPermissions.Users.ChangePassword,
            "تغییر رمز عبور", // Change Password
            "تغییر رمز عبور کاربران" // Change user passwords
        );

        users.AddChild(
            UserManagementPermissions.Users.ResetPassword,
            "بازنشانی رمز عبور", // Reset Password
            "بازنشانی رمز عبور کاربران" // Reset user passwords
        );

        users.AddChild(
            UserManagementPermissions.Users.Lock,
            "قفل کردن کاربر", // Lock User
            "قفل کردن دسترسی کاربر به سیستم" // Lock user access to the system
        );

        users.AddChild(
            UserManagementPermissions.Users.Unlock,
            "باز کردن قفل کاربر", // Unlock User
            "باز کردن قفل دسترسی کاربر به سیستم" // Unlock user access to the system
        );

        users.AddChild(
            UserManagementPermissions.Users.Activate,
            "فعال‌سازی کاربر", // Activate User
            "فعال‌سازی حساب کاربری" // Activate user account
        );

        users.AddChild(
            UserManagementPermissions.Users.Deactivate,
            "غیرفعال‌سازی کاربر", // Deactivate User
            "غیرفعال‌سازی حساب کاربری" // Deactivate user account
        );

        users.AddChild(
            UserManagementPermissions.Users.ManageRoles,
            "مدیریت نقش‌های کاربر", // Manage User Roles
            "تخصیص و حذف نقش‌ها به/از کاربران" // Assign and remove roles to/from users
        );

        users.AddChild(
            UserManagementPermissions.Users.ManageClaims,
            "مدیریت ادعاهای کاربر", // Manage User Claims
            "مدیریت ادعاها و دسترسی‌های خاص کاربران" // Manage user-specific claims and permissions
        );
    }
}
