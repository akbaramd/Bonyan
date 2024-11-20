using Bonyan.Layer.Domain.DomainService;
using Bonyan.UserManagement.Domain.Users.DomainServices;

namespace Bonyan.IdentityManagement.Domain.Users.DomainServices;

public interface IBonIdentityUserManager<TIdentityUser> : IBonUserManager<TIdentityUser>
    where TIdentityUser : IBonIdentityUser
{
    Task<BonDomainResult> AssignRoleAsync(TIdentityUser user, string roleName);
    Task<BonDomainResult> RemoveRoleAsync(TIdentityUser user, string roleName);
    Task<BonDomainResult<IReadOnlyList<string>>> GetUserRolesAsync(TIdentityUser user);
    
    Task<BonDomainResult> CreateAsync(TIdentityUser entity, string password);
    Task<BonDomainResult> ChangePasswordAsync(TIdentityUser entity, string currentPassword, string newPassword);
    Task<BonDomainResult> ResetPasswordAsync(TIdentityUser entity, string newPassword);
    
    
    // Token-related behaviors
    Task<BonDomainResult> SetTokenAsync(TIdentityUser user, string tokenType, string tokenValue, DateTime? expiration = null);
    Task<BonDomainResult> RemoveTokenAsync(TIdentityUser user, string tokenType);
    Task<BonDomainResult<TIdentityUser>> FindByTokenAsync(string tokenType, string tokenValue);

}

