namespace Bonyan.IdentityManagement.Permissions;

/// <summary>
/// Permission definition provider for Identity Management module
/// </summary>
public class BonIdentityManagementPermissionDefinitionProvider : IBonPermissionDefinitionProvider
{
    public void Define(IPermissionDefinitionContext context)
    {
        var identityManagementGroup = context.AddGroup(
            BonIdentityManagementPermissions.GroupName,
            "مدیریت هویت و دسترسی", // Identity & Access Management
            "مدیریت کاربران، نقش‌ها، مجوزها و احراز هویت" // User, role, permission and authentication management
        );

        DefineUserPermissions(identityManagementGroup);
        DefineRolePermissions(identityManagementGroup);
        DefineClaimsPermissions(identityManagementGroup);
        DefineAuthenticationPermissions(identityManagementGroup);
        DefineAdministrationPermissions(identityManagementGroup);
    }

    private static void DefineUserPermissions(PermissionGroupDefinition group)
    {
        var users = group.AddPermission(
            BonIdentityManagementPermissions.Users.Default,
            "مدیریت کاربران", // User Management
            "دسترسی عمومی به بخش مدیریت کاربران" // General access to user management section
        );

        users.AddChild(
            BonIdentityManagementPermissions.Users.View,
            "مشاهده کاربران", // View Users
            "مشاهده لیست کاربران و اطلاعات آن‌ها" // View user list and their information
        );

        users.AddChild(
            BonIdentityManagementPermissions.Users.Create,
            "ایجاد کاربر", // Create User
            "ایجاد کاربر جدید در سیستم" // Create new user in the system
        );

        users.AddChild(
            BonIdentityManagementPermissions.Users.Edit,
            "ویرایش کاربر", // Edit User
            "ویرایش اطلاعات کاربران موجود" // Edit existing user information
        );

        users.AddChild(
            BonIdentityManagementPermissions.Users.Delete,
            "حذف کاربر", // Delete User
            "حذف کاربران از سیستم" // Delete users from the system
        );

        users.AddChild(
            BonIdentityManagementPermissions.Users.ManageRoles,
            "مدیریت نقش‌های کاربر", // Manage User Roles
            "تخصیص و حذف نقش‌ها به/از کاربران" // Assign and remove roles to/from users
        );

        users.AddChild(
            BonIdentityManagementPermissions.Users.ManageClaims,
            "مدیریت ادعاهای کاربر", // Manage User Claims
            "مدیریت ادعاها و دسترسی‌های خاص کاربران" // Manage user-specific claims and permissions
        );

        users.AddChild(
            BonIdentityManagementPermissions.Users.ChangePassword,
            "تغییر رمز عبور", // Change Password
            "تغییر رمز عبور کاربران" // Change user passwords
        );

        users.AddChild(
            BonIdentityManagementPermissions.Users.ResetPassword,
            "بازنشانی رمز عبور", // Reset Password
            "بازنشانی رمز عبور کاربران" // Reset user passwords
        );

        users.AddChild(
            BonIdentityManagementPermissions.Users.BanUser,
            "مسدود کردن کاربر", // Ban User
            "مسدود کردن دسترسی کاربر به سیستم" // Block user access to the system
        );

        users.AddChild(
            BonIdentityManagementPermissions.Users.ActivateUser,
            "فعال‌سازی کاربر", // Activate User
            "فعال‌سازی یا غیرفعال‌سازی حساب کاربری" // Activate or deactivate user account
        );
    }

    private static void DefineRolePermissions(PermissionGroupDefinition group)
    {
        var roles = group.AddPermission(
            BonIdentityManagementPermissions.Roles.Default,
            "مدیریت نقش‌ها", // Role Management
            "دسترسی عمومی به بخش مدیریت نقش‌ها" // General access to role management section
        );

        roles.AddChild(
            BonIdentityManagementPermissions.Roles.View,
            "مشاهده نقش‌ها", // View Roles
            "مشاهده لیست نقش‌ها و اطلاعات آن‌ها" // View role list and their information
        );

        roles.AddChild(
            BonIdentityManagementPermissions.Roles.Create,
            "ایجاد نقش", // Create Role
            "ایجاد نقش جدید در سیستم" // Create new role in the system
        );

        roles.AddChild(
            BonIdentityManagementPermissions.Roles.Edit,
            "ویرایش نقش", // Edit Role
            "ویرایش اطلاعات نقش‌های موجود" // Edit existing role information
        );

        roles.AddChild(
            BonIdentityManagementPermissions.Roles.Delete,
            "حذف نقش", // Delete Role
            "حذف نقش‌ها از سیستم" // Delete roles from the system
        );

        roles.AddChild(
            BonIdentityManagementPermissions.Roles.ManagePermissions,
            "مدیریت مجوزهای نقش", // Manage Role Permissions
            "تخصیص و حذف مجوزها به/از نقش‌ها" // Assign and remove permissions to/from roles
        );

        roles.AddChild(
            BonIdentityManagementPermissions.Roles.ManageClaims,
            "مدیریت ادعاهای نقش", // Manage Role Claims
            "مدیریت ادعاها و دسترسی‌های خاص نقش‌ها" // Manage role-specific claims and permissions
        );

        roles.AddChild(
            BonIdentityManagementPermissions.Roles.AssignToUsers,
            "تخصیص نقش به کاربر", // Assign Role to User
            "تخصیص نقش‌ها به کاربران" // Assign roles to users
        );
    }

    private static void DefineClaimsPermissions(PermissionGroupDefinition group)
    {
        var claims = group.AddPermission(
            BonIdentityManagementPermissions.Claims.Default,
            "مدیریت ادعاها", // Claims Management
            "دسترسی عمومی به بخش مدیریت ادعاها" // General access to claims management section
        );

        claims.AddChild(
            BonIdentityManagementPermissions.Claims.ViewClaims,
            "مشاهده ادعاها", // View Claims
            "مشاهده لیست ادعاها و اطلاعات آن‌ها" // View claims list and their information
        );

        claims.AddChild(
            BonIdentityManagementPermissions.Claims.ManageUserClaims,
            "مدیریت ادعاهای کاربر", // Manage User Claims
            "مدیریت ادعاهای اختصاصی کاربران" // Manage user-specific claims
        );

        claims.AddChild(
            BonIdentityManagementPermissions.Claims.ManageRoleClaims,
            "مدیریت ادعاهای نقش", // Manage Role Claims
            "مدیریت ادعاهای اختصاصی نقش‌ها" // Manage role-specific claims
        );
    }

    private static void DefineAuthenticationPermissions(PermissionGroupDefinition group)
    {
        var auth = group.AddPermission(
            BonIdentityManagementPermissions.Authentication.Default,
            "مدیریت احراز هویت", // Authentication Management
            "دسترسی عمومی به بخش مدیریت احراز هویت" // General access to authentication management section
        );

        auth.AddChild(
            BonIdentityManagementPermissions.Authentication.ManageTokens,
            "مدیریت توکن‌ها", // Manage Tokens
            "مدیریت توکن‌های احراز هویت" // Manage authentication tokens
        );

        auth.AddChild(
            BonIdentityManagementPermissions.Authentication.ViewLoginHistory,
            "مشاهده تاریخچه ورود", // View Login History
            "مشاهده تاریخچه ورود کاربران" // View user login history
        );
    }

    private static void DefineAdministrationPermissions(PermissionGroupDefinition group)
    {
        var admin = group.AddPermission(
            BonIdentityManagementPermissions.Administration.Default,
            "مدیریت سیستم", // System Administration
            "دسترسی عمومی به بخش مدیریت سیستم" // General access to system administration section
        );

        admin.AddChild(
            BonIdentityManagementPermissions.Administration.ManageSystem,
            "مدیریت کلی سیستم", // Manage System
            "دسترسی کامل به مدیریت سیستم" // Full system management access
        );

        admin.AddChild(
            BonIdentityManagementPermissions.Administration.ViewSystemLogs,
            "مشاهده لاگ‌های سیستم", // View System Logs
            "مشاهده گزارش‌ها و لاگ‌های سیستم" // View system reports and logs
        );

        admin.AddChild(
            BonIdentityManagementPermissions.Administration.SystemConfiguration,
            "تنظیمات سیستم", // System Configuration
            "دسترسی به تنظیمات عمومی سیستم" // Access to general system settings
        );
    }
} 