using Bonyan.IdentityManagement.Domain.Permissions;

namespace Bonyan.AspNetCore.Authorization.Permissions;

public interface IBonPermissionManager
{
    public IEnumerable<BonIdentityPermission> GetAllPermissions();
}