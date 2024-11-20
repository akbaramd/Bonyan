using Bonyan.Layer.Domain.DomainService;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.Domain.Users.Enumerations;

namespace Bonyan.UserManagement.Domain.Users.DomainServices;

/// <summary>
/// Manages the lifecycle and operations for user entities.
/// </summary>
public interface IBonUserManager<TUser> : IBonDomainService where TUser : IBonUser
{
    // Core CRUD operations
    Task<BonDomainResult> CreateAsync(TUser entity);
    Task<BonDomainResult> UpdateAsync(TUser entity);
    Task<BonDomainResult<TUser>> FindByIdAsync(BonUserId id);
    Task<BonDomainResult<TUser>> FindByUserNameAsync(string userName);
    Task<BonDomainResult<TUser>> FindByPhoneNumberAsync(string phoneNumber);
    Task<BonDomainResult<TUser>> FindByPhoneNumberAsync(BonUserPhoneNumber phoneNumber);
    Task<BonDomainResult<TUser>> FindByEmailAsync(string email);
    Task<BonDomainResult<TUser>> FindByEmailAsync(BonUserEmail email);

    // Verification methods
    Task<BonDomainResult> VerifyEmailAsync(TUser user);
    Task<BonDomainResult> VerifyPhoneNumberAsync(TUser user);

    // Status management
    Task<BonDomainResult> ActivateUserAsync(TUser user);
    Task<BonDomainResult> DeactivateUserAsync(TUser user);
    Task<BonDomainResult> SuspendUserAsync(TUser user);
    Task<BonDomainResult> ChangeUserStatusAsync(TUser user, UserStatus newStatus);

    // Security
}
