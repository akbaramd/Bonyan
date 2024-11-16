using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.Layer.Domain.DomainService;

namespace Bonyan.IdentityManagement.Domain.Abstractions.Roles;

public interface IBonIdentityRoleManager
{
    Task<BonDomainResult> CreateAsync(string name, string title);
    Task<BonDomainResult> UpdateAsync(BonIdentityRole entity);
    Task<BonDomainResult<BonIdentityRole>> FindByNameAsync(string name);
    Task<BonDomainResult> DeleteAsync(BonIdentityRole identityRole);
    Task<BonDomainResult<bool>> RoleExistsAsync(string roleName);
}