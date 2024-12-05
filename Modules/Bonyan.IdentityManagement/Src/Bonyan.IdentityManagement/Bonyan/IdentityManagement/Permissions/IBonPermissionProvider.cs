using Bonyan.IdentityManagement.Domain.Permissions;

namespace Bonyan.AspNetCore.Authorization.Permissions;

public interface IBonPermissionProvider
{
    public BonIdentityPermission[] GetPermissions();
}