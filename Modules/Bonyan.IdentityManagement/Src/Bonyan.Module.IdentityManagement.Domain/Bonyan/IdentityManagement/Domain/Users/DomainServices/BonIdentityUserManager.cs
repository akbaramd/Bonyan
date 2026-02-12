using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repositories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Domain.DomainService;
using Microsoft.Extensions.Logging;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain.Services;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.Layer.Domain.Exceptions;

namespace Bonyan.IdentityManagement.Domain.Users.DomainServices;

/// <summary>
/// Domain service for the final identity user (non-generic).
/// </summary>
public class BonIdentityUserManager : BonDomainService, IBonIdentityUserManager
{
    public IBonIdentityUserRepository UserRepository =>
        LazyServiceProvider.LazyGetRequiredService<IBonIdentityUserRepository>();

    public IBonIdentityRoleRepository RoleRepository =>
        LazyServiceProvider.LazyGetRequiredService<IBonIdentityRoleRepository>();

    private IUserTokenHasher? TokenHasher => LazyServiceProvider.LazyGetService<IUserTokenHasher>();

    private async Task<BonIdentityRole?> FindRoleAsync(BonRoleId roleId)
    {
        if (roleId == null) throw new BonDomainException("Role ID cannot be null.", "NullRoleId");
        try
        {
            return await RoleRepository.FindOneAsync(x => x.Id == roleId);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding role.");
            return null;
        }
    }

    public async Task<BonDomainResult<BonIdentityUser>> CreateAsync(BonIdentityUser entity)
    {
        if (entity == null) throw new BonDomainException("User entity cannot be null.", "NullEntity");
        try
        {
            if (await UserRepository.ExistsAsync(x => x.UserName.Equals(entity.UserName)))
            {
                Logger.LogWarning("User with username {UserName} already exists.", entity.UserName);
                return BonDomainResult<BonIdentityUser>.Failure($"User with username {entity.UserName} already exists.");
            }
            entity.Id = BonUserId.NewId();
            var res = await UserRepository.AddAsync(entity, true);
            return BonDomainResult<BonIdentityUser>.Success(res);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error creating user.");
            return BonDomainResult<BonIdentityUser>.Failure("Error creating user.");
        }
    }

    public async Task<BonDomainResult<BonIdentityUser>> UpdateAsync(BonIdentityUser entity)
    {
        if (entity == null) throw new BonDomainException("User entity cannot be null.", "NullEntity");
        try
        {
            await UserRepository.UpdateAsync(entity, true);
            return BonDomainResult<BonIdentityUser>.Success(entity);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error updating user.");
            return BonDomainResult<BonIdentityUser>.Failure("Error updating user.");
        }
    }

    public async Task<BonDomainResult<BonIdentityUser>> FindByIdAsync(BonUserId id)
    {
        if (id == null) throw new BonDomainException("User ID cannot be null.", "NullId");
        try
        {
            var user = await UserRepository.FindOneAsync(x => x.Id == id);
            if (user == null)
                return BonDomainResult<BonIdentityUser>.Failure($"User with ID {id} not found.");
            return BonDomainResult<BonIdentityUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by ID.");
            return BonDomainResult<BonIdentityUser>.Failure("Error finding user by ID.");
        }
    }

    public async Task<BonDomainResult<BonIdentityUser>> FindByUserNameAsync(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName)) throw new BonDomainException("Username cannot be null or empty.", "NullOrEmptyUserName");
        try
        {
            var user = await UserRepository.FindOneAsync(x => x.UserName.Equals(userName));
            if (user == null)
                return BonDomainResult<BonIdentityUser>.Failure($"User with username {userName} not found.");
            return BonDomainResult<BonIdentityUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by username.");
            return BonDomainResult<BonIdentityUser>.Failure("Error finding user by username.");
        }
    }

    public async Task<BonDomainResult<BonIdentityUser>> FindByPhoneNumberAsync(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) throw new BonDomainException("Phone number cannot be null or empty.", "NullOrEmptyPhoneNumber");
        try
        {
            var user = await UserRepository.FindOneAsync(x => x.PhoneNumber != null && x.PhoneNumber.Number.Equals(phoneNumber));
            if (user == null)
                return BonDomainResult<BonIdentityUser>.Failure($"User with phone number {phoneNumber} not found.");
            return BonDomainResult<BonIdentityUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by phone number.");
            return BonDomainResult<BonIdentityUser>.Failure("Error finding user by phone number.");
        }
    }

    public async Task<BonDomainResult<BonIdentityUser>> FindByPhoneNumberAsync(BonUserPhoneNumber phoneNumber)
    {
        try
        {
            var user = await UserRepository.FindOneAsync(x => x.PhoneNumber != null && x.PhoneNumber.Equals(phoneNumber));
            if (user == null)
                return BonDomainResult<BonIdentityUser>.Failure($"User with phone number {phoneNumber} not found.");
            return BonDomainResult<BonIdentityUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by phone number.");
            return BonDomainResult<BonIdentityUser>.Failure("Error finding user by phone number.");
        }
    }

    public async Task<BonDomainResult<BonIdentityUser>> FindByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new BonDomainException("Email cannot be null or empty.", "NullOrEmptyEmail");
        try
        {
            var user = await UserRepository.FindOneAsync(x => x.Email != null && x.Email.Address.Equals(email));
            if (user == null)
                return BonDomainResult<BonIdentityUser>.Failure($"User with email {email} not found.");
            return BonDomainResult<BonIdentityUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by email.");
            return BonDomainResult<BonIdentityUser>.Failure("Error finding user by email.");
        }
    }

    public async Task<BonDomainResult<BonIdentityUser>> FindByEmailAsync(BonUserEmail email)
    {
        try
        {
            var user = await UserRepository.FindOneAsync(x => x.Email != null && x.Email.Equals(email));
            if (user == null)
                return BonDomainResult<BonIdentityUser>.Failure($"User with email {email} not found.");
            return BonDomainResult<BonIdentityUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by email.");
            return BonDomainResult<BonIdentityUser>.Failure("Error finding user by email.");
        }
    }

    public async Task<BonDomainResult> VerifyEmailAsync(BonIdentityUser user)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        try
        {
            if (user.Email == null)
                return BonDomainResult.Failure("Email address is not set.");
            user.VerifyEmail();
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error verifying email.");
            return BonDomainResult.Failure("Error verifying email.");
        }
    }

    public async Task<BonDomainResult> VerifyPhoneNumberAsync(BonIdentityUser user)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        try
        {
            if (user.PhoneNumber == null)
                return BonDomainResult.Failure("Phone number is not set.");
            user.VerifyPhoneNumber();
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error verifying phone number.");
            return BonDomainResult.Failure("Error verifying phone number.");
        }
    }

    public Task<BonDomainResult> ActivateUserAsync(BonIdentityUser user) => ChangeUserStatusAsync(user, UserStatus.Active);
    public Task<BonDomainResult> DeactivateUserAsync(BonIdentityUser user) => ChangeUserStatusAsync(user, UserStatus.Deactivated);
    public Task<BonDomainResult> SuspendUserAsync(BonIdentityUser user) => ChangeUserStatusAsync(user, UserStatus.Suspended);

    public async Task<BonDomainResult> ChangeUserStatusAsync(BonIdentityUser user, UserStatus newStatus)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        try
        {
            user.ChangeStatus(newStatus);
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error changing user status.");
            return BonDomainResult.Failure("Error changing user status.");
        }
    }

    public async Task<BonDomainResult<BonIdentityUser>> AssignRolesAsync(BonIdentityUser user, IEnumerable<BonRoleId> roleIds)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (roleIds == null || !roleIds.Any()) throw new BonDomainException("RoleIds cannot be null or empty.", "NullOrEmptyRoleIds");
        try
        {
            var roles = await RoleRepository.FindAsync(r => roleIds.Contains(r.Id));
            var roleIdsFound = roles.Select(r => r.Id).ToHashSet();
            foreach (var roleId in roleIds)
            {
                if (!roleIdsFound.Contains(roleId)) continue;
                try { user.AssignRole(roleId); } catch (InvalidOperationException) { }
            }
            await UserRepository.UpdateAsync(user, true);
            return BonDomainResult<BonIdentityUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error updating roles for user {UserId}.", user.Id);
            return BonDomainResult<BonIdentityUser>.Failure("Error updating roles.");
        }
    }

    public async Task<BonDomainResult<IReadOnlyList<BonIdentityRole>>> GetUserRolesAsync(BonIdentityUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        try
        {
            var roleIds = user.UserRoles.Select(x => x.RoleId).ToList();
            if (roleIds.Count == 0)
                return BonDomainResult<IReadOnlyList<BonIdentityRole>>.Success(Array.Empty<BonIdentityRole>().AsReadOnly());
            var roles = await RoleRepository.FindAsync(r => roleIds.Contains(r.Id));
            return BonDomainResult<IReadOnlyList<BonIdentityRole>>.Success(roles.ToList().AsReadOnly());
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error fetching user roles.");
            return BonDomainResult<IReadOnlyList<BonIdentityRole>>.Failure("Error fetching user roles.");
        }
    }

    public async Task<BonDomainResult> RemoveRoleAsync(BonIdentityUser user, BonRoleId roleId)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (roleId == null) throw new BonDomainException("RoleId cannot be null.", "NullRoleId");
        try
        {
            user.RemoveRole(roleId);
            await UserRepository.UpdateAsync(user, true);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error removing role from user {UserId}.", user.Id);
            return BonDomainResult.Failure("Error removing role from user.");
        }
    }

    public async Task<BonDomainResult<BonIdentityUser>> CreateAsync(BonIdentityUser entity, string password)
    {
        try
        {
            entity.SetPassword(password);
            return await CreateAsync(entity);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error creating user.");
            return BonDomainResult<BonIdentityUser>.Failure("Error creating user.");
        }
    }

    public async Task<BonDomainResult> ChangePasswordAsync(BonIdentityUser user, string currentPassword, string newPassword)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(currentPassword)) throw new BonDomainException("Current password cannot be null.", "NullOrEmptyCurrentPassword");
        if (string.IsNullOrWhiteSpace(newPassword)) throw new BonDomainException("New password cannot be null.", "NullOrEmptyNewPassword");
        try
        {
            if (!user.VerifyPassword(currentPassword))
                return BonDomainResult.Failure("Current password does not match.");
            user.SetPassword(newPassword);
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error changing password for user {UserId}.", user.Id);
            return BonDomainResult.Failure("Error changing password.");
        }
    }

    public async Task<BonDomainResult> ResetPasswordAsync(BonIdentityUser user, string newPassword)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(newPassword)) throw new BonDomainException("New password cannot be null.", "NullOrEmptyNewPassword");
        try
        {
            user.SetPassword(newPassword);
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error resetting password for user {UserId}.", user.Id);
            return BonDomainResult.Failure("Error resetting password.");
        }
    }

    public async Task<BonDomainResult> SetTokenAsync(BonIdentityUser user, string tokenType, string tokenValue, DateTime? expiration = null)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(tokenType)) throw new BonDomainException("TokenType is required.", "NullOrEmptyTokenType");
        if (string.IsNullOrWhiteSpace(tokenValue)) throw new BonDomainException("TokenValue is required.", "NullOrEmptyTokenValue");
        try
        {
            var valueToStore = TokenHasher != null ? TokenHasher.Hash(tokenValue) : tokenValue;
            user.SetToken(tokenType, valueToStore, expiration);
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error setting token for user {UserId}.", user.Id);
            return BonDomainResult.Failure("Error setting token for user.");
        }
    }

    public async Task<BonDomainResult> RemoveTokenAsync(BonIdentityUser user, string tokenType)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(tokenType)) throw new BonDomainException("TokenType is required.", "NullOrEmptyTokenType");
        try
        {
            user.RemoveToken(tokenType);
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error removing token for user {UserId}.", user.Id);
            return BonDomainResult.Failure("Error removing token for user.");
        }
    }

    public async Task<BonDomainResult<BonIdentityUser>> FindByTokenAsync(string tokenType, string tokenValue)
    {
        if (string.IsNullOrWhiteSpace(tokenType)) throw new BonDomainException("TokenType is required.", "NullOrEmptyTokenType");
        if (string.IsNullOrWhiteSpace(tokenValue)) throw new BonDomainException("TokenValue is required.", "NullOrEmptyTokenValue");
        try
        {
            var valueToMatch = TokenHasher != null ? TokenHasher.Hash(tokenValue) : tokenValue;
            var user = await UserRepository.FindOneAsync(u => u.Tokens.Any(t => t.Type == tokenType && t.Value == valueToMatch));
            if (user == null)
                return BonDomainResult<BonIdentityUser>.Failure("No user found with the specified token.");
            return BonDomainResult<BonIdentityUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by token.");
            return BonDomainResult<BonIdentityUser>.Failure("Error finding user by token.");
        }
    }

    public async Task<BonDomainResult> AddClaimAsync(BonIdentityUser user, string claimType, string claimValue, string? issuer = null)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(claimType)) throw new BonDomainException("Claim type cannot be null or empty.", "InvalidClaimType");
        if (string.IsNullOrWhiteSpace(claimValue)) throw new BonDomainException("Claim value cannot be null or empty.", "InvalidClaimValue");
        try
        {
            if (user.HasClaim(claimType, claimValue))
                return BonDomainResult.Failure($"User already has claim '{claimType}' with value '{claimValue}'.");
            user.AddClaim(BonUserClaimId.NewId(), claimType, claimValue, issuer);
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error adding claim to user {UserId}.", user.Id);
            return BonDomainResult.Failure("Error adding claim.");
        }
    }

    public async Task<BonDomainResult> RemoveClaimAsync(BonIdentityUser user, string claimType, string claimValue)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(claimType)) throw new BonDomainException("Claim type cannot be null or empty.", "InvalidClaimType");
        if (string.IsNullOrWhiteSpace(claimValue)) throw new BonDomainException("Claim value cannot be null or empty.", "InvalidClaimValue");
        try
        {
            if (!user.HasClaim(claimType, claimValue))
                return BonDomainResult.Failure($"User does not have claim '{claimType}' with value '{claimValue}'.");
            user.RemoveClaim(claimType, claimValue);
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error removing claim from user {UserId}.", user.Id);
            return BonDomainResult.Failure("Error removing claim.");
        }
    }

    public async Task<BonDomainResult> RemoveClaimsByTypeAsync(BonIdentityUser user, string claimType)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(claimType)) throw new BonDomainException("Claim type cannot be null or empty.", "InvalidClaimType");
        try
        {
            user.RemoveClaimsByType(claimType);
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error removing claims by type from user {UserId}.", user.Id);
            return BonDomainResult.Failure("Error removing claims by type.");
        }
    }

    public Task<BonDomainResult<bool>> HasClaimAsync(BonIdentityUser user, string claimType, string claimValue)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(claimType)) throw new BonDomainException("Claim type cannot be null or empty.", "InvalidClaimType");
        if (string.IsNullOrWhiteSpace(claimValue)) throw new BonDomainException("Claim value cannot be null or empty.", "InvalidClaimValue");
        try
        {
            return Task.FromResult(BonDomainResult<bool>.Success(user.HasClaim(claimType, claimValue)));
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error checking claim for user {UserId}.", user.Id);
            return Task.FromResult(BonDomainResult<bool>.Failure("Error checking claim."));
        }
    }

    public Task<BonDomainResult<IEnumerable<BonIdentityUserClaims>>> GetClaimsByTypeAsync(BonIdentityUser user, string claimType)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(claimType)) throw new BonDomainException("Claim type cannot be null or empty.", "InvalidClaimType");
        try
        {
            var claims = user.GetClaimsByType(claimType);
            return Task.FromResult(BonDomainResult<IEnumerable<BonIdentityUserClaims>>.Success(claims));
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error getting claims by type for user {UserId}.", user.Id);
            return Task.FromResult(BonDomainResult<IEnumerable<BonIdentityUserClaims>>.Failure("Error getting claims by type."));
        }
    }

    public Task<BonDomainResult<IEnumerable<BonIdentityUserClaims>>> GetAllClaimsAsync(BonIdentityUser user)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        try
        {
            return Task.FromResult(BonDomainResult<IEnumerable<BonIdentityUserClaims>>.Success(user.UserClaims));
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error getting all claims for user {UserId}.", user.Id);
            return Task.FromResult(BonDomainResult<IEnumerable<BonIdentityUserClaims>>.Failure("Error getting all claims."));
        }
    }

    public async Task<BonDomainResult> AddPermissionAsync(BonIdentityUser user, string permissionName, string? issuer = null)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(permissionName)) throw new BonDomainException("Permission name cannot be null or empty.", "InvalidPermissionName");
        try
        {
            if (user.HasPermission(permissionName))
                return BonDomainResult.Failure($"User already has permission '{permissionName}'.");
            user.AddPermission(permissionName, issuer);
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error adding permission to user {UserId}.", user.Id);
            return BonDomainResult.Failure("Error adding permission.");
        }
    }

    public async Task<BonDomainResult> RemovePermissionAsync(BonIdentityUser user, string permissionName)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(permissionName)) throw new BonDomainException("Permission name cannot be null or empty.", "InvalidPermissionName");
        try
        {
            if (!user.HasPermission(permissionName))
                return BonDomainResult.Failure($"User does not have permission '{permissionName}'.");
            user.RemovePermission(permissionName);
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error removing permission from user {UserId}.", user.Id);
            return BonDomainResult.Failure("Error removing permission.");
        }
    }

    public Task<BonDomainResult<bool>> HasPermissionAsync(BonIdentityUser user, string permissionName)
    {
        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(permissionName)) throw new BonDomainException("Permission name cannot be null or empty.", "InvalidPermissionName");
        try
        {
            return Task.FromResult(BonDomainResult<bool>.Success(user.HasPermission(permissionName)));
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error checking permission for user {UserId}.", user.Id);
            return Task.FromResult(BonDomainResult<bool>.Failure("Error checking permission."));
        }
    }

    public async Task<BonDomainResult> DeleteAsync(BonIdentityUser user)
    {
        if (user == null)
            return BonDomainResult.Failure("User cannot be null.");
        if (user.IsDeleted)
            return BonDomainResult.Failure("User is already deleted.");
        try
        {
            await UserRepository.DeleteAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting user {UserId}.", user.Id);
            return BonDomainResult.Failure("Error deleting user.");
        }
    }
}
