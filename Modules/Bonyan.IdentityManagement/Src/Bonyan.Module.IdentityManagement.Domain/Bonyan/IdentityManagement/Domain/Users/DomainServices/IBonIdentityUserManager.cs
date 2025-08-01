﻿using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users.DomainServices;

public interface IBonIdentityUserManager<TIdentityUser,TRole> : IBonDomainService
    where TIdentityUser : BonIdentityUser<TIdentityUser,TRole> where TRole : BonIdentityRole<TRole>
{
    
    Task<BonDomainResult<TIdentityUser>> CreateAsync(TIdentityUser entity);
    Task<BonDomainResult<TIdentityUser>> UpdateAsync(TIdentityUser entity);
    Task<BonDomainResult<TIdentityUser>> FindByIdAsync(BonUserId id);
    Task<BonDomainResult<TIdentityUser>> FindByUserNameAsync(string userName);
    Task<BonDomainResult<TIdentityUser>> FindByPhoneNumberAsync(string phoneNumber);
    Task<BonDomainResult<TIdentityUser>> FindByPhoneNumberAsync(BonUserPhoneNumber phoneNumber);
    Task<BonDomainResult<TIdentityUser>> FindByEmailAsync(string email);
    Task<BonDomainResult<TIdentityUser>> FindByEmailAsync(BonUserEmail email);

    // Verification methods
    Task<BonDomainResult> VerifyEmailAsync(TIdentityUser user);
    Task<BonDomainResult> VerifyPhoneNumberAsync(TIdentityUser user);

    // Status management
    Task<BonDomainResult> ActivateUserAsync(TIdentityUser user);
    Task<BonDomainResult> DeactivateUserAsync(TIdentityUser user);
    Task<BonDomainResult> SuspendUserAsync(TIdentityUser user);
    Task<BonDomainResult> ChangeUserStatusAsync(TIdentityUser user, UserStatus newStatus);
    
    Task<BonDomainResult<TIdentityUser>> AssignRolesAsync(TIdentityUser user, IEnumerable<BonRoleId> roleIds);
    Task<BonDomainResult> RemoveRoleAsync(TIdentityUser user, BonRoleId roleName);
    Task<BonDomainResult<IReadOnlyList<TRole>>> GetUserRolesAsync(TIdentityUser user);
    
    Task<BonDomainResult<TIdentityUser>> CreateAsync(TIdentityUser entity, string password);
    Task<BonDomainResult> ChangePasswordAsync(TIdentityUser entity, string currentPassword, string newPassword);
    Task<BonDomainResult> ResetPasswordAsync(TIdentityUser entity, string newPassword);
    
    
    // Token-related behaviors
    Task<BonDomainResult> SetTokenAsync(TIdentityUser user, string tokenType, string tokenValue, DateTime? expiration = null);
    Task<BonDomainResult> RemoveTokenAsync(TIdentityUser user, string tokenType);
    Task<BonDomainResult<TIdentityUser>> FindByTokenAsync(string tokenType, string tokenValue);

    // Claims-related behaviors
    Task<BonDomainResult> AddClaimAsync(TIdentityUser user, string claimType, string claimValue, string? issuer = null);
    Task<BonDomainResult> RemoveClaimAsync(TIdentityUser user, string claimType, string claimValue);
    Task<BonDomainResult> RemoveClaimsByTypeAsync(TIdentityUser user, string claimType);
    Task<BonDomainResult<bool>> HasClaimAsync(TIdentityUser user, string claimType, string claimValue);
    Task<BonDomainResult<IEnumerable<BonIdentityUserClaims<TIdentityUser,TRole>>>> GetClaimsByTypeAsync(TIdentityUser user, string claimType);
    Task<BonDomainResult<IEnumerable<BonIdentityUserClaims<TIdentityUser,TRole>>>> GetAllClaimsAsync(TIdentityUser user);

}

