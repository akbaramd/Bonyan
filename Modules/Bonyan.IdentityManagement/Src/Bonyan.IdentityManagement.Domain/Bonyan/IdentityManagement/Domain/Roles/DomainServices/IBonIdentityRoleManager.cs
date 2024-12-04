using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.Layer.Domain.DomainService;

namespace Bonyan.IdentityManagement.Domain.Roles.DomainServices
{
    public interface IBonIdentityRoleManager
    {
        Task<BonDomainResult> CreateRoleAsync(BonIdentityRole role);
        Task<BonDomainResult> CreateRoleWithPermissionsAsync(BonIdentityRole role, IEnumerable<BonPermissionId> permissionIds);
        Task<BonDomainResult> UpdateRoleAsync(BonIdentityRole role);
        Task<BonDomainResult> DeleteRoleAsync(BonIdentityRole role);
        Task<BonDomainResult> AssignPermissionsToRoleAsync(BonIdentityRole role, IEnumerable<BonPermissionId> permissionIds);
        Task<BonDomainResult<BonIdentityRole>> FindRoleByKeyAsync(string roleKey);
    }
}