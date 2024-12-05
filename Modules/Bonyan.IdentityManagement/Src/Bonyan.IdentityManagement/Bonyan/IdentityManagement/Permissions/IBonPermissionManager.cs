using Bonyan.IdentityManagement.Domain.Permissions;

namespace Bonyan.IdentityManagement.Permissions;

public interface IBonPermissionManager
{
    public IEnumerable<BonIdentityPermission> GetAllPermissions();
}