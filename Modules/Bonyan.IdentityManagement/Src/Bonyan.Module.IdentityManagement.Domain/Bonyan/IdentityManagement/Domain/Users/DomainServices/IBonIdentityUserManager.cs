using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users.DomainServices;

/// <summary>
/// Domain service for the final identity user (non-generic).
/// </summary>
public interface IBonIdentityUserManager : IBonDomainService
{
    Task<BonDomainResult<BonIdentityUser>> CreateAsync(BonIdentityUser entity);
    Task<BonDomainResult<BonIdentityUser>> CreateAsync(BonIdentityUser entity, string password);
    Task<BonDomainResult<BonIdentityUser>> UpdateAsync(BonIdentityUser entity);
    Task<BonDomainResult<BonIdentityUser>> FindByIdAsync(BonUserId id);
    Task<BonDomainResult<BonIdentityUser>> FindByUserNameAsync(string userName);
    Task<BonDomainResult<BonIdentityUser>> FindByPhoneNumberAsync(string phoneNumber);
    Task<BonDomainResult<BonIdentityUser>> FindByPhoneNumberAsync(BonUserPhoneNumber phoneNumber);
    Task<BonDomainResult<BonIdentityUser>> FindByEmailAsync(string email);
    Task<BonDomainResult<BonIdentityUser>> FindByEmailAsync(BonUserEmail email);

    Task<BonDomainResult> VerifyEmailAsync(BonIdentityUser user);
    Task<BonDomainResult> VerifyPhoneNumberAsync(BonIdentityUser user);

    Task<BonDomainResult> ActivateUserAsync(BonIdentityUser user);
    Task<BonDomainResult> DeactivateUserAsync(BonIdentityUser user);
    Task<BonDomainResult> SuspendUserAsync(BonIdentityUser user);
    Task<BonDomainResult> ChangeUserStatusAsync(BonIdentityUser user, UserStatus newStatus);

    Task<BonDomainResult<BonIdentityUser>> AssignRolesAsync(BonIdentityUser user, IEnumerable<BonRoleId> roleIds);
    Task<BonDomainResult> RemoveRoleAsync(BonIdentityUser user, BonRoleId roleId);
    Task<BonDomainResult<IReadOnlyList<BonIdentityRole>>> GetUserRolesAsync(BonIdentityUser user);

    Task<BonDomainResult> ChangePasswordAsync(BonIdentityUser entity, string currentPassword, string newPassword);
    Task<BonDomainResult> ResetPasswordAsync(BonIdentityUser entity, string newPassword);

    Task<BonDomainResult> SetTokenAsync(BonIdentityUser user, string tokenType, string tokenValue, DateTime? expiration = null);
    Task<BonDomainResult> RemoveTokenAsync(BonIdentityUser user, string tokenType);
    Task<BonDomainResult<BonIdentityUser>> FindByTokenAsync(string tokenType, string tokenValue);

    Task<BonDomainResult> AddClaimAsync(BonIdentityUser user, string claimType, string claimValue, string? issuer = null);
    Task<BonDomainResult> RemoveClaimAsync(BonIdentityUser user, string claimType, string claimValue);
    Task<BonDomainResult> RemoveClaimsByTypeAsync(BonIdentityUser user, string claimType);
    Task<BonDomainResult<bool>> HasClaimAsync(BonIdentityUser user, string claimType, string claimValue);
    Task<BonDomainResult<IEnumerable<BonIdentityUserClaims>>> GetClaimsByTypeAsync(BonIdentityUser user, string claimType);
    Task<BonDomainResult<IEnumerable<BonIdentityUserClaims>>> GetAllClaimsAsync(BonIdentityUser user);

    Task<BonDomainResult> AddPermissionAsync(BonIdentityUser user, string permissionName, string? issuer = null);
    Task<BonDomainResult> RemovePermissionAsync(BonIdentityUser user, string permissionName);
    Task<BonDomainResult<bool>> HasPermissionAsync(BonIdentityUser user, string permissionName);

    Task<BonDomainResult> DeleteAsync(BonIdentityUser user);
}
