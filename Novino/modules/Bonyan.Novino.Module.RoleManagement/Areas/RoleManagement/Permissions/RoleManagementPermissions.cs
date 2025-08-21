 using Bonyan.IdentityManagement.Permissions;

namespace Bonyan.Novino.Module.RoleManagement.Areas.RoleManagement.Permissions;

/// <summary>
/// Permission constants for Role Management module
/// </summary>
public static class RoleManagementPermissions
{
    public const string GroupName = "RoleManagement";

    public static class Roles
    {
        public const string Default = GroupName + ".Roles";
        public const string View = Default + ".View";
        public const string Details = Default + ".Details";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Export = Default + ".Export";
        public const string BulkDelete = Default + ".BulkDelete";
        public const string BulkActivate = Default + ".BulkActivate";
        public const string BulkDeactivate = Default + ".BulkDeactivate";
        public const string ManageClaims = Default + ".ManageClaims";
        public const string ManageUsers = Default + ".ManageUsers";
        public const string AssignToUser = Default + ".AssignToUser";
        public const string RemoveFromUser = Default + ".RemoveFromUser";
        public const string ViewUsers = Default + ".ViewUsers";
    }
}

/// <summary>
/// Permission definition provider for Role Management module
/// </summary>
public class RoleManagementPermissionProvider : IBonPermissionDefinitionProvider
{
    public void Define(IPermissionDefinitionContext context)
    {
        var roleManagementGroup = context.AddGroup(
            RoleManagementPermissions.GroupName,
            "مدیریت نقش‌ها", // Role Management
            "دسترسی‌های لازم برای مدیریت نقش‌ها" // Permissions required for role management
        );

        DefineRolePermissions(roleManagementGroup);
    }

    private static void DefineRolePermissions(PermissionGroupDefinition group)
    {
        var roles = group.AddPermission(
            RoleManagementPermissions.Roles.Default,
            "مدیریت نقش‌ها", // Role Management
            "دسترسی عمومی به بخش مدیریت نقش‌ها" // General access to role management section
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.View,
            "مشاهده نقش‌ها", // View Roles
            "مشاهده لیست نقش‌ها و اطلاعات آن‌ها" // View role list and their information
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.Details,
            "جزئیات نقش", // Role Details
            "مشاهده جزئیات کامل نقش و اطلاعات مربوطه" // View complete role details and related information
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.Create,
            "ایجاد نقش", // Create Role
            "ایجاد نقش جدید در سیستم" // Create new role in the system
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.Edit,
            "ویرایش نقش", // Edit Role
            "ویرایش اطلاعات نقش‌های موجود" // Edit existing role information
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.Delete,
            "حذف نقش", // Delete Role
            "حذف نقش‌ها از سیستم" // Delete roles from the system
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.Export,
            "خروجی نقش‌ها", // Export Roles
            "خروجی گرفتن از لیست نقش‌ها" // Export role list
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.BulkDelete,
            "حذف گروهی نقش‌ها", // Bulk Delete Roles
            "حذف همزمان چندین نقش" // Delete multiple roles at once
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.BulkActivate,
            "فعال‌سازی گروهی نقش‌ها", // Bulk Activate Roles
            "فعال‌سازی همزمان چندین نقش" // Activate multiple roles at once
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.BulkDeactivate,
            "غیرفعال‌سازی گروهی نقش‌ها", // Bulk Deactivate Roles
            "غیرفعال‌سازی همزمان چندین نقش" // Deactivate multiple roles at once
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.ManageClaims,
            "مدیریت ادعاهای نقش", // Manage Role Claims
            "مدیریت ادعاها و دسترسی‌های نقش‌ها" // Manage role claims and permissions
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.ManageUsers,
            "مدیریت کاربران نقش", // Manage Role Users
            "مدیریت کاربران تخصیص داده شده به نقش‌ها" // Manage users assigned to roles
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.AssignToUser,
            "تخصیص نقش به کاربر", // Assign Role to User
            "تخصیص نقش به کاربران" // Assign roles to users
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.RemoveFromUser,
            "حذف نقش از کاربر", // Remove Role from User
            "حذف نقش از کاربران" // Remove roles from users
        );

        roles.AddChild(
            RoleManagementPermissions.Roles.ViewUsers,
            "مشاهده کاربران نقش", // View Role Users
            "مشاهده کاربران تخصیص داده شده به نقش" // View users assigned to role
        );
    }
}