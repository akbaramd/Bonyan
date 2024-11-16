using Bonyan.Layer.Domain.DomainService;
using Bonyan.UserManagement.Domain.Users.DomainServices;
using Bonyan.UserManagement.Domain.Users.Entities;

namespace Bonyan.IdentityManagement.Domain.Users;

public interface IBonIdentityUserManager<TIdentityUser> : IBonUserManager<TIdentityUser>
    where TIdentityUser : IBonIdentityUser
{
    Task<BonDomainResult> AssignRoleAsync(TIdentityUser user, string roleName);
    Task<BonDomainResult> RemoveRoleAsync(TIdentityUser user, string roleName);
    Task<BonDomainResult<IReadOnlyList<string>>> GetUserRolesAsync(TIdentityUser user);
}