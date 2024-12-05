using Bonyan.IdentityManagement.Domain.Permissions;

namespace Bonyan.IdentityManagement.Permissions;

public interface IBonPermissionProvider
{
    public BonIdentityPermission[] GetPermissions();
}