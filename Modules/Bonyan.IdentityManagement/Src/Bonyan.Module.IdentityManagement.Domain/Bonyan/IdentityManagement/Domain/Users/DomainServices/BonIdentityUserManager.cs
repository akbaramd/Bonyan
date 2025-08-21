using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Domain.DomainService;
using Microsoft.Extensions.Logging;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain.Services;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using System.Diagnostics.CodeAnalysis;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.Layer.Domain.Exceptions;

namespace Bonyan.IdentityManagement.Domain.Users.DomainServices;

public class BonIdentityUserManager<TUser,TRole> : BonDomainService, IBonIdentityUserManager<TUser,TRole>
    where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
    
    
    
    
        // Create a new user
    public async Task<BonDomainResult<TUser>> CreateAsync(TUser entity)
    {
        if (entity == null) throw new BonDomainException("User entity cannotbe null.", "NullEntity");

        try
        {
            if (await UserRepository.ExistsAsync(x => x.UserName.Equals(entity.UserName)))
            {
                Logger.LogWarning($"User with username {entity.UserName} already exists.");
                return BonDomainResult<TUser>.Failure($"User with username {entity.UserName} already exists.");
            }

            entity.Id = BonUserId.NewId();
           var res = await UserRepository.AddAsync(entity, true);
            return BonDomainResult<TUser>.Success(res);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error creating user.");
            return BonDomainResult<TUser>.Failure("Error creating user.");
        }
    }

    // Update user information
    public async Task<BonDomainResult<TUser>> UpdateAsync(TUser entity)
    {
        if (entity == null) throw new BonDomainException("User entity cannotbe null.", "NullEntity");

        try
        {
            await UserRepository.UpdateAsync(entity, true);
            return BonDomainResult<TUser>.Success(entity);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error updating user.");
            return BonDomainResult<TUser>.Failure("Error updating user.");
        }
    }
    // Find user by username
    public async Task<BonDomainResult<TUser>> FindByIdAsync(BonUserId id)
    {
        if (id == null) throw new BonDomainException("User ID cannotbe null.", "NullId");

        try
        {
            var user = await UserRepository.FindOneAsync(x => x.Id == id);
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with ID {id} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by ID.");
            return BonDomainResult<TUser>.Failure("Error finding user by ID.");
        }
    }
    // Find user by username
    public async Task<BonDomainResult<TUser>> FindByUserNameAsync(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName)) throw new BonDomainException("Username cannot be null or empty.", "NullOrEmptyUserName");

        try
        {
            var user = await UserRepository.FindOneAsync(x => x.UserName.Equals(userName));
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with username {userName} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by username.");
            return BonDomainResult<TUser>.Failure("Error finding user by username.");
        }
    }

    // Find user by phone number (string)
    public async Task<BonDomainResult<TUser>> FindByPhoneNumberAsync(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) throw new BonDomainException("Phone number cannot be null or empty.", "NullOrEmptyPhoneNumber");

        try
        {
            var user = await UserRepository.FindOneAsync(x =>
                x.PhoneNumber != null && x.PhoneNumber.Number.Equals(phoneNumber));
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with phone number {phoneNumber} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by phone number.");
            return BonDomainResult<TUser>.Failure("Error finding user by phone number.");
        }
    }

    // Find user by phone number (value object)
    public async Task<BonDomainResult<TUser>> FindByPhoneNumberAsync(BonUserPhoneNumber phoneNumber)
    {
        try
        {
            var user = await UserRepository.FindOneAsync(
                x => x.PhoneNumber != null && x.PhoneNumber.Equals(phoneNumber));
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with phone number {phoneNumber} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by phone number.");
            return BonDomainResult<TUser>.Failure("Error finding user by phone number.");
        }
    }

    // Find user by email (string)
    public async Task<BonDomainResult<TUser>> FindByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new BonDomainException("Email cannot be null or empty.", "NullOrEmptyEmail");

        try
        {
            var user = await UserRepository.FindOneAsync(x => x.Email != null && x.Email.Address.Equals(email));
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with email {email} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by email.");
            return BonDomainResult<TUser>.Failure("Error finding user by email.");
        }
    }

    // Find user by email (value object)
    public async Task<BonDomainResult<TUser>> FindByEmailAsync(BonUserEmail email)
    {
        try
        {
            var user = await UserRepository.FindOneAsync(x => x.Email != null && x.Email.Equals(email));
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with email {email} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by email.");
            return BonDomainResult<TUser>.Failure("Error finding user by email.");
        }
    }

    // Verify email
    public async Task<BonDomainResult> VerifyEmailAsync(TUser user)
    {
        if (user == null) throw new BonDomainException("User cannot benull.", "NullUser");

        try
        {
            if (user.Email == null)
            {
                return BonDomainResult.Failure("Email address is not set.");
            }

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

    // Verify phone number
    public async Task<BonDomainResult> VerifyPhoneNumberAsync(TUser user)
    {
        if (user == null) throw new BonDomainException("User cannot benull.", "NullUser");

        try
        {
            if (user.PhoneNumber == null)
            {
                return BonDomainResult.Failure("Phone number is not set.");
            }

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

    // Activate user
    public async Task<BonDomainResult> ActivateUserAsync(TUser user)
    {
        return await ChangeUserStatusAsync(user, UserStatus.Active);
    }

    // Deactivate user
    public async Task<BonDomainResult> DeactivateUserAsync(TUser user)
    {
        return await ChangeUserStatusAsync(user, UserStatus.Deactivated);
    }

    public async Task<BonDomainResult> SuspendUserAsync(TUser user)
    {
        return await ChangeUserStatusAsync(user, UserStatus.Suspended);
    }


    // Change user status
    public async Task<BonDomainResult> ChangeUserStatusAsync(TUser user, UserStatus newStatus)
    {
        if (user == null) throw new BonDomainException("User cannot benull.", "NullUser");

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
    
    public IBonIdentityUserRepository<TUser,TRole> UserRepository =>
        LazyServiceProvider.LazyGetRequiredService<IBonIdentityUserRepository<TUser,TRole>>();


    public IBonIdentityRoleRepository<TRole> RoleRepository =>
        LazyServiceProvider.LazyGetRequiredService<IBonIdentityRoleRepository<TRole>>();

    public IBonIdentityUserClaimsRepository<TUser,TRole> UserClaimsRepository =>
        LazyServiceProvider.LazyGetRequiredService<IBonIdentityUserClaimsRepository<TUser,TRole>>();

    private async Task<TRole?> FindRoleAsync(BonRoleId roleId)
    {
        if (roleId == null) throw new BonDomainException("Role IDcannot be null.", "NullRoleId");

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

    public async Task<BonDomainResult<TUser>> AssignRolesAsync(TUser user, IEnumerable<BonRoleId> roleIds)
    {
        const string methodName = nameof(AssignRolesAsync);

        if (user == null) throw new BonDomainException("User cannot benull.", "NullUser");
        if (roleIds == null || !roleIds.Any()) throw new BonDomainException("RoleIds cannot be null or empty.", "NullOrEmptyRoleIds");

        try
        {
            // Fetch all roles in one go to validate existence
            var roles = await RoleRepository.FindAsync(r => roleIds.Contains(r.Id));
            var roleIdsFound = roles.Select(r => r.Id).ToHashSet();

            // Add roles to the user entity (logic is encapsulated in the entity)
            foreach (var roleId in roleIds)
            {
                if (!roleIdsFound.Contains(roleId))
                {
                    Logger.LogWarning($"{methodName}: Role with ID {roleId.Value} not found.");
                    continue;
                }

                try
                {
                    user.AssignRole(roleId); // Call the domain entity method
                }
                catch (InvalidOperationException ex)
                {
                    Logger.LogWarning($"{methodName}: {ex.Message}");
                }
            }

            // Persist user changes
            await UserRepository.UpdateAsync(user, true);

            Logger.LogInformation($"{methodName}: Successfully updated roles for user {user.Id}.");
            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error updating roles for user {user.Id}.");
            return BonDomainResult<TUser>.Failure("Error updating roles.");
        }
    }


    // Optimized GetUserRolesAsync with better caching of roles
    public Task<BonDomainResult<IReadOnlyList<TRole>>> GetUserRolesAsync(TUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        try
        {
            return Task.FromResult(BonDomainResult<IReadOnlyList<TRole>>.Success(user.UserRoles.Select(x=>x.Role).OfType<TRole>().ToList().AsReadOnly()));
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error fetching user roles.");
            return Task.FromResult(BonDomainResult<IReadOnlyList<TRole>>.Failure("Error fetching user roles."));
        }
    }

    // Create user with optimized role assignment logic
    public async Task<BonDomainResult<TUser>> CreateAsync(TUser entity, string password, IEnumerable<BonRoleId> roleIds)
    {
        const string methodName = nameof(CreateAsync);

        try
        {
            entity.SetPassword(password);
            var result = await CreateAsync(entity);
            if (!result.IsSuccess)
            {
                return result;
            }

            // Batch role assignment
            var roleAssignmentResult = await AssignRolesAsync(entity, roleIds);
            if (!roleAssignmentResult.IsSuccess)
            {
                return roleAssignmentResult;
            }

            Logger.LogInformation($"{methodName}: Successfully created user {entity.Id}.");
            return BonDomainResult<TUser>.Success(roleAssignmentResult.Value);
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error creating user.");
            return BonDomainResult<TUser>.Failure("Error creating user.");
        }
    }

    // Optimize role removal with batch operations
    public async Task<BonDomainResult> RemoveRoleAsync(TUser user, BonRoleId roleId)
    {
        const string methodName = nameof(RemoveRoleAsync);

        if (user == null) throw new BonDomainException("User cannot benull.", "NullUser");
        if (roleId == null) throw new BonDomainException("RoleIdcannot be null.", "NullRoleId");

        try
        {
            user.RemoveRole(roleId);
            await UserRepository.DeleteAsync(user, true);

            Logger.LogInformation($"{methodName}: Successfully removed role {roleId.Value} from user {user.Id}.");
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error removing role from user {user.Id}.");
            return BonDomainResult.Failure("Error removing role from user.");
        }
    }

    // Create a new user and set password
    public async Task<BonDomainResult<TUser>> CreateAsync(TUser entity, string password)
    {
        const string methodName = nameof(CreateAsync);

        try
        {
            entity.SetPassword(password);
            var result = await CreateAsync(entity);
            Logger.LogInformation($"{methodName}: Successfully created user {entity.Id}.");
            return result;
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error creating user.");
            return BonDomainResult<TUser>.Failure("Error creating user.");
        }
    }

    // Change a user's password
    public async Task<BonDomainResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword)
    {
        const string methodName = nameof(ChangePasswordAsync);

        if (user == null) throw new BonDomainException("User cannot benull.", "NullUser");
        if (string.IsNullOrWhiteSpace(currentPassword)) throw new BonDomainException("Current password cannot be null.", "NullOrEmptyCurrentPassword");
        if (string.IsNullOrWhiteSpace(newPassword)) throw new BonDomainException("New password cannot be null.", "NullOrEmptyNewPassword");

        try
        {
            if (!user.VerifyPassword(currentPassword))
            {
                Logger.LogWarning($"{methodName}: Current password does not match for user {user.Id}.");
                return BonDomainResult.Failure("Current password does not match.");
            }

            user.SetPassword(newPassword);
            await UpdateAsync(user);

            Logger.LogInformation($"{methodName}: Successfully changed password for user {user.Id}.");
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error changing password for user {user.Id}.");
            return BonDomainResult.Failure("Error changing password.");
        }
    }

    // Reset a user's password directly (e.g., for admin use cases)
    public async Task<BonDomainResult> ResetPasswordAsync(TUser user, string newPassword)
    {
        const string methodName = nameof(ResetPasswordAsync);

        if (user == null) throw new BonDomainException("User cannot benull.", "NullUser");
        if (string.IsNullOrWhiteSpace(newPassword)) throw new BonDomainException("New password cannot be null.", "NullOrEmptyNewPassword");

        try
        {
            user.SetPassword(newPassword);
            await UpdateAsync(user);

            Logger.LogInformation($"{methodName}: Successfully reset password for user {user.Id}.");
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error resetting password for user {user.Id}.");
            return BonDomainResult.Failure("Error resetting password.");
        }
    }

    // Add a token to the user
    public async Task<BonDomainResult> SetTokenAsync(TUser user, string tokenType, string tokenValue, DateTime? expiration = null)
    {
        const string methodName = nameof(SetTokenAsync);

        if (user == null) throw new BonDomainException("User cannot benull.", "NullUser");
        if (string.IsNullOrWhiteSpace(tokenType)) throw new BonDomainException("TokenType is required.", "NullOrEmptyTokenType");
        if (string.IsNullOrWhiteSpace(tokenValue)) throw new BonDomainException("TokenValue is required.", "NullOrEmptyTokenValue");

        try
        {
            user.SetToken(tokenType, tokenValue, expiration);
            await UpdateAsync(user);

            Logger.LogInformation($"{methodName}: Successfully set token {tokenType} for user {user.Id}.");
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error setting token for user {user.Id}.");
            return BonDomainResult.Failure("Error setting token for user.");
        }
    }

    // Remove a token from the user
    public async Task<BonDomainResult> RemoveTokenAsync(TUser user, string tokenType)
    {
        const string methodName = nameof(RemoveTokenAsync);

        if (user == null) throw new BonDomainException("User cannot benull.", "NullUser");
        if (string.IsNullOrWhiteSpace(tokenType)) throw new BonDomainException("TokenType is required.", "NullOrEmptyTokenType");

        try
        {
            user.RemoveToken(tokenType);
            await UpdateAsync(user);

            Logger.LogInformation($"{methodName}: Successfully removed token {tokenType} for user {user.Id}.");
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error removing token for user {user.Id}.");
            return BonDomainResult.Failure("Error removing token for user.");
        }
    }

    // Find a user by token
    public async Task<BonDomainResult<TUser>> FindByTokenAsync(string tokenType, string tokenValue)
    {
        const string methodName = nameof(FindByTokenAsync);

        if (string.IsNullOrWhiteSpace(tokenType)) throw new BonDomainException("TokenType is required.", "NullOrEmptyTokenType");
        if (string.IsNullOrWhiteSpace(tokenValue)) throw new BonDomainException("TokenValue is required.", "NullOrEmptyTokenValue");

        try
        {
            var user = await UserRepository.FindOneAsync(u => u.Tokens.Any(t => t.Type == tokenType && t.Value == tokenValue));
            if (user == null)
            {
                Logger.LogWarning($"{methodName}: No user found with token {tokenType}.");
                return BonDomainResult<TUser>.Failure("No user found with the specified token.");
            }

            Logger.LogInformation($"{methodName}: Successfully found user {user.Id} by token {tokenType}.");
            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error finding user by token.");
            return BonDomainResult<TUser>.Failure("Error finding user by token.");
        }
    }

    public async Task<BonDomainResult> BanUserAsync(TUser user, DateTime until)
    {
        const string methodName = nameof(BanUserAsync);

        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (until <= DateTime.Now) throw new BonDomainException("Ban date must be in the future.", "InvalidBanDate");

        try
        {
            user.LockAccountUntil(until);
            await UpdateAsync(user);

            Logger.LogInformation($"{methodName}: Successfully banned user {user.Id} until {until}.");
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error banning user {user.Id}.");
            return BonDomainResult.Failure("Error banning user.");
        }
    }

    // Unban a user
    public async Task<BonDomainResult> UnbanUserAsync(TUser user)
    {
        const string methodName = nameof(UnbanUserAsync);

        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");

        try
        {
            user.UnlockAccount();
            await UpdateAsync(user);

            Logger.LogInformation($"{methodName}: Successfully unbanned user {user.Id}.");
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error unbanning user {user.Id}.");
            return BonDomainResult.Failure("Error unbanning user.");
        }
    }

    // Additional user management behavior
    public async Task<BonDomainResult> UpdateUserProfileAsync(TUser user, UserProfile profile)
    {
        const string methodName = nameof(UpdateUserProfileAsync);

        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (profile == null) throw new BonDomainException("Profile cannot be null.", "NullProfile");

        try
        {
            user.UpdateProfile(profile);
            await UpdateAsync(user);

            Logger.LogInformation($"{methodName}: Successfully updated profile for user {user.Id}.");
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error updating profile for user {user.Id}.");
            return BonDomainResult.Failure("Error updating profile.");
        }
    }

    // Claims-related behaviors
    public async Task<BonDomainResult> AddClaimAsync(TUser user, string claimType, string claimValue, string? issuer = null)
    {
        const string methodName = nameof(AddClaimAsync);

        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(claimType)) 
            throw new BonDomainException("Claim type cannot be null or empty.", "InvalidClaimType");
        if (string.IsNullOrWhiteSpace(claimValue)) 
            throw new BonDomainException("Claim value cannot be null or empty.", "InvalidClaimValue");

        try
        {
            // Check if user already has this claim
            if (user.HasClaim(claimType, claimValue))
            {
                Logger.LogWarning($"{methodName}: User {user.Id} already has claim '{claimType}' with value '{claimValue}'.");
                return BonDomainResult.Failure($"User already has claim '{claimType}' with value '{claimValue}'.");
            }

            // Use domain behavior to add claim
            user.AddClaim(BonUserClaimId.NewId(), claimType, claimValue, issuer);
            await UpdateAsync(user);

            Logger.LogInformation($"{methodName}: Successfully added claim '{claimType}' with value '{claimValue}' to user {user.Id}.");
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error adding claim to user {user.Id}.");
            return BonDomainResult.Failure("Error adding claim.");
        }
    }

    public async Task<BonDomainResult> RemoveClaimAsync(TUser user, string claimType, string claimValue)
    {
        const string methodName = nameof(RemoveClaimAsync);

        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(claimType)) 
            throw new BonDomainException("Claim type cannot be null or empty.", "InvalidClaimType");
        if (string.IsNullOrWhiteSpace(claimValue)) 
            throw new BonDomainException("Claim value cannot be null or empty.", "InvalidClaimValue");

        try
        {
            // Check if user has this claim
            if (!user.HasClaim(claimType, claimValue))
            {
                Logger.LogWarning($"{methodName}: User {user.Id} does not have claim '{claimType}' with value '{claimValue}'.");
                return BonDomainResult.Failure($"User does not have claim '{claimType}' with value '{claimValue}'.");
            }

            // Use domain behavior to remove claim
            user.RemoveClaim(claimType, claimValue);
            await UpdateAsync(user);

            Logger.LogInformation($"{methodName}: Successfully removed claim '{claimType}' with value '{claimValue}' from user {user.Id}.");
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error removing claim from user {user.Id}.");
            return BonDomainResult.Failure("Error removing claim.");
        }
    }

    public async Task<BonDomainResult> RemoveClaimsByTypeAsync(TUser user, string claimType)
    {
        const string methodName = nameof(RemoveClaimsByTypeAsync);

        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(claimType)) 
            throw new BonDomainException("Claim type cannot be null or empty.", "InvalidClaimType");

        try
        {
            // Use domain behavior to remove claims by type
            user.RemoveClaimsByType(claimType);
            await UpdateAsync(user);

            Logger.LogInformation($"{methodName}: Successfully removed all claims of type '{claimType}' from user {user.Id}.");
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error removing claims by type from user {user.Id}.");
            return BonDomainResult.Failure("Error removing claims by type.");
        }
    }

    public async Task<BonDomainResult<bool>> HasClaimAsync(TUser user, string claimType, string claimValue)
    {
        const string methodName = nameof(HasClaimAsync);

        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(claimType)) 
            throw new BonDomainException("Claim type cannot be null or empty.", "InvalidClaimType");
        if (string.IsNullOrWhiteSpace(claimValue)) 
            throw new BonDomainException("Claim value cannot be null or empty.", "InvalidClaimValue");

        try
        {
            var hasClaim = user.HasClaim(claimType, claimValue);
            Logger.LogInformation($"{methodName}: User {user.Id} {(hasClaim ? "has" : "does not have")} claim '{claimType}' with value '{claimValue}'.");
            return BonDomainResult<bool>.Success(hasClaim);
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error checking claim for user {user.Id}.");
            return BonDomainResult<bool>.Failure("Error checking claim.");
        }
    }

    public async Task<BonDomainResult<IEnumerable<BonIdentityUserClaims<TUser,TRole>>>> GetClaimsByTypeAsync(TUser user, string claimType)
    {
        const string methodName = nameof(GetClaimsByTypeAsync);

        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");
        if (string.IsNullOrWhiteSpace(claimType)) 
            throw new BonDomainException("Claim type cannot be null or empty.", "InvalidClaimType");

        try
        {
            var claims = user.GetClaimsByType(claimType);
            Logger.LogInformation($"{methodName}: Retrieved {claims.Count()} claims of type '{claimType}' for user {user.Id}.");
            return BonDomainResult<IEnumerable<BonIdentityUserClaims<TUser,TRole>>>.Success(claims);
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error getting claims by type for user {user.Id}.");
            return BonDomainResult<IEnumerable<BonIdentityUserClaims<TUser,TRole>>>.Failure("Error getting claims by type.");
        }
    }

    public async Task<BonDomainResult<IEnumerable<BonIdentityUserClaims<TUser,TRole>>>> GetAllClaimsAsync(TUser user)
    {
        const string methodName = nameof(GetAllClaimsAsync);

        if (user == null) throw new BonDomainException("User cannot be null.", "NullUser");

        try
        {
            var claims = user.UserClaims;
            Logger.LogInformation($"{methodName}: Retrieved {claims.Count()} total claims for user {user.Id}.");
            return BonDomainResult<IEnumerable<BonIdentityUserClaims<TUser,TRole>>>.Success(claims);
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error getting all claims for user {user.Id}.");
            return BonDomainResult<IEnumerable<BonIdentityUserClaims<TUser,TRole>>>.Failure("Error getting all claims.");
        }
    }

    public async Task<BonDomainResult> DeleteAsync(TUser user)
    {
        const string methodName = nameof(DeleteAsync);

        if (user == null)
        {
            Logger.LogWarning($"{methodName}: User cannot be null.");
            return BonDomainResult.Failure("User cannot be null.");
        }

        try
        {
            // If the user is already deleted or marked as deleted, handle accordingly
            if (user.IsDeleted)
            {
                Logger.LogInformation($"{methodName}: User {user.Id} is already deleted.");
                return BonDomainResult.Failure("User is already deleted.");
            }


            // Save changes to the repository/unit of work
            await UserRepository.DeleteAsync(user);

            Logger.LogInformation($"{methodName}: User {user.Id} marked as deleted successfully.");
            return BonDomainResult.Success();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{methodName}: Error deleting user {user?.Id}.");
            return BonDomainResult.Failure("Error deleting user.");
        }
    }
}
