using Bonyan.IdentityManagement.Permissions;

namespace Bonyan.Novino.Web.Permissions
{
    /// <summary>
    /// Permission definition provider for user management
    /// </summary>
    public class UserManagementPermissionDefinitionProvider : IBonPermissionDefinitionProvider
    {
        public void Define(IPermissionDefinitionContext context)
        {
            var userManagementGroup = context.AddGroup(
                name: "UserManagement",
                displayName: "مدیریت کاربران",
                description: "دسترسی‌های مربوط به مدیریت کاربران سیستم"
            );

            // Basic user management permissions
            userManagementGroup.AddPermission(
                name: "UserManagement.Users.Read",
                displayName: "مشاهده کاربران",
                description: "امکان مشاهده لیست کاربران"
            );

            userManagementGroup.AddPermission(
                name: "UserManagement.Users.Create",
                displayName: "ایجاد کاربر",
                description: "امکان ایجاد کاربر جدید"
            );

            userManagementGroup.AddPermission(
                name: "UserManagement.Users.Detail",
                displayName: "جزئیات کاربر",
                description: "امکان مشاهده جزئیات کامل کاربر"
            );

            userManagementGroup.AddPermission(
                name: "UserManagement.Users.Delete",
                displayName: "حذف کاربر",
                description: "امکان حذف کاربر"
            );

            userManagementGroup.AddPermission(
                name: "UserManagement.Users.Update",
                displayName: "ویرایش کاربر",
                description: "امکان ویرایش اطلاعات کاربر"
            );

            userManagementGroup.AddPermission(
                name: "UserManagement.Users.ChangePassword",
                displayName: "تغییر رمز عبور",
                description: "امکان تغییر رمز عبور کاربر"
            );

            // User role management permissions
            userManagementGroup.AddPermission(
                name: "UserManagement.Users.RoleManagement",
                displayName: "مدیریت نقش کاربران",
                description: "امکان تخصیص و حذف نقش از کاربران"
            );
        }
    }
} 