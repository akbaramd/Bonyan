using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Permissions;

namespace Bonyan.IdentityManagement.WebApi;

public class BonIdentityPermissionProvider : IBonPermissionProvider
{
    public BonIdentityPermission[] GetPermissions()
    {
        return new[]
        {
            BonIdentityPermission.Create(
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityPermissionRead),
                "خواندن دسترسی‌ها"
            ),
            BonIdentityPermission.Create(
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityRoleRead),
                "خواندن نقش‌ها"
            ),
            BonIdentityPermission.Create(
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityRoleDelete),
                "حذف نقش‌ها"
            ),
            BonIdentityPermission.Create(
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityRoleEdit),
                "ویرایش نقش‌ها"
            ),
            BonIdentityPermission.Create(
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityRoleCreate),
                "ایجاد نقش‌ها"
            ),
            BonIdentityPermission.Create(
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityUserRead),
                "خواندن کاربران"
            ),
            BonIdentityPermission.Create(
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityUserDelete),
                "حذف کاربران"
            ),
            BonIdentityPermission.Create(
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityUserEdit),
                "ویرایش کاربران"
            ),
            BonIdentityPermission.Create(
                BonPermissionId.NewId(BonIdentityPermissionConstants.IdentityUserCreate),
                "ایجاد کاربران"
            )
        };
    }
}