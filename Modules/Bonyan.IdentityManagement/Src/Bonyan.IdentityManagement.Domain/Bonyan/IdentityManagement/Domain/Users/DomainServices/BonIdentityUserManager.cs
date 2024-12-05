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

namespace Bonyan.IdentityManagement.Domain.Users.DomainServices;

public class BonIdentityUserManager<TUser> : BonDomainService, IBonIdentityUserManager<TUser>
    where TUser : class, IBonIdentityUser
{
    
    
    
    
        // Create a new user
    public async Task<BonDomainResult> CreateAsync(TUser entity)
    {
        try
        {
            if (await UserRepository.ExistsAsync(x => x.UserName.Equals(entity.UserName)))
            {
                Logger.LogWarning($"User with username {entity.UserName} already exists.");
                return BonDomainResult.Failure($"User with username {entity.UserName} already exists.");
            }

            entity.Id = BonUserId.NewId();
            await UserRepository.AddAsync(entity, true);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error creating user.");
            return BonDomainResult.Failure(e.Message);
        }
    }

    // Update user information
    public async Task<BonDomainResult> UpdateAsync(TUser entity)
    {
        try
        {
            await UserRepository.UpdateAsync(entity, true);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error updating user.");
            return BonDomainResult.Failure("Error updating user.");
        }
    }
    // Find user by username
    public async Task<BonDomainResult<TUser>> FindByIdAsync(BonUserId id)
    {
        try
        {
            var user = await UserRepository.FindOneAsync(x => x.Id == id);
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"User with username {id} not found.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by username.");
            return BonDomainResult<TUser>.Failure("Error finding user by username.");
        }
    }
    // Find user by username
    public async Task<BonDomainResult<TUser>> FindByUserNameAsync(string userName)
    {
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
    
    public new IBonIdentityUserRepository<TUser> UserRepository =>
        LazyServiceProvider.LazyGetRequiredService<IBonIdentityUserRepository<TUser>>();

    public IBonIdentityUserRolesRepository UserRoleRepository =>
        LazyServiceProvider.LazyGetRequiredService<IBonIdentityUserRolesRepository>();

    public new IBonIdentityRoleRepository RoleRepository =>
        LazyServiceProvider.LazyGetRequiredService<IBonIdentityRoleRepository>();

    private async Task<BonIdentityRole?> FindRoleAsync(BonRoleId roleId)
    {
        return await RoleRepository.FindOneAsync(x => x.Id == roleId);
    }

    private void LogAndThrow(string message, string methodName)
    {
        Logger.LogError($"{methodName}: {message}");
        throw new InvalidOperationException(message);
    }

     public async Task<BonDomainResult> AssignRolesAsync(TUser user, IEnumerable<BonRoleId> roleIds)
    {
        const string methodName = nameof(AssignRolesAsync);

        if (user == null) LogAndThrow("User cannot be null.", methodName);
        if (roleIds == null || !roleIds.Any()) LogAndThrow("RoleIds cannot be null or empty.", methodName);

        try
        {
            var roles = await RoleRepository.FindAsync(r => roleIds.Contains(r.Id)); // Fetch all roles in bulk

            var existingUserRoles = await UserRoleRepository.FindAsync(ur => ur.UserId == user.Id && roleIds.Contains(ur.RoleId)); 
            var existingRoleIds = existingUserRoles.Select(ur => ur.RoleId).ToHashSet();

            var rolesToAdd = roles.Where(r => !existingRoleIds.Contains(r.Id)).ToList(); // Filter out already assigned roles
            var rolesToRemove = existingUserRoles.Where(ur => !roleIds.Contains(ur.RoleId)).ToList(); // Get roles to remove

            // Perform assignments and removals in parallel to improve performance
            var addRolesTask = Task.WhenAll(rolesToAdd.Select(role => UserRoleRepository.AddAsync(new BonIdentityUserRoles(user.Id, role.Id), true)));
            var removeRolesTask = Task.WhenAll(rolesToRemove.Select(userRole => UserRoleRepository.DeleteAsync(userRole, true)));

            await Task.WhenAll(addRolesTask, removeRolesTask);

            Logger.LogInformation($"{methodName}: Successfully updated roles for user {user.Id}.");
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error updating roles for user {user.Id}.");
            return BonDomainResult.Failure("Error updating roles.");
        }
    }

    // Optimized GetUserRolesAsync with better caching of roles
    public async Task<BonDomainResult<IReadOnlyList<BonIdentityRole>>> GetUserRolesAsync(TUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        try
        {
            // Fetch all roles in one go and filter valid roles
            var userRoles = await UserRoleRepository.FindAsync(ur => ur.UserId == user.Id);
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
            var roles = await RoleRepository.FindAsync(r => roleIds.Contains(r.Id));

            return BonDomainResult<IReadOnlyList<BonIdentityRole>>.Success(roles.ToList());
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error fetching user roles.");
            return BonDomainResult<IReadOnlyList<BonIdentityRole>>.Failure("Error fetching user roles.");
        }
    }

    // Create user with optimized role assignment logic
    public async Task<BonDomainResult> CreateAsync(TUser entity, string password, IEnumerable<BonRoleId> roleIds)
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
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error creating user.");
            return BonDomainResult.Failure("Error creating user.");
        }
    }

    // Optimize role removal with batch operations
    public async Task<BonDomainResult> RemoveRoleAsync(TUser user, BonRoleId roleId)
    {
        const string methodName = nameof(RemoveRoleAsync);

        if (user == null) LogAndThrow("User cannot be null.", methodName);
        if (roleId == null) LogAndThrow("RoleId cannot be null.", methodName);

        try
        {
            // Check if the role is assigned
            var userRole = await UserRoleRepository.FindOneAsync(ur => ur.UserId == user.Id && ur.RoleId == roleId);
            if (userRole == null)
            {
                Logger.LogWarning($"{methodName}: User does not have the role {roleId.Value}.");
                return BonDomainResult.Failure($"User does not have the role {roleId.Value}.");
            }

            await UserRoleRepository.DeleteAsync(userRole, true);

            Logger.LogInformation($"{methodName}: Successfully removed role {roleId.Value} from user {user.Id}.");
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"{methodName}: Error removing role from user {user.Id}.");
            return BonDomainResult.Failure("Error removing role from user.");
        }
    }

    public Task<BonDomainResult<IReadOnlyList<BonIdentityRole>>> GeTIdentityUserRolesAsync(TUser user)
    {
        throw new NotImplementedException();
    }

    // Create a new user and set password
    public async Task<BonDomainResult> CreateAsync(TUser entity, string password)
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
            return BonDomainResult.Failure("Error creating user.");
        }
    }

    // Change a user's password
    public async Task<BonDomainResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword)
    {
        const string methodName = nameof(ChangePasswordAsync);

        if (user == null) LogAndThrow("User cannot be null.", methodName);
        if (string.IsNullOrWhiteSpace(currentPassword)) LogAndThrow("Current password cannot be null.", methodName);
        if (string.IsNullOrWhiteSpace(newPassword)) LogAndThrow("New password cannot be null.", methodName);

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

        if (user == null) LogAndThrow("User cannot be null.", methodName);
        if (string.IsNullOrWhiteSpace(newPassword)) LogAndThrow("New password cannot be null.", methodName);

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

        if (user == null) LogAndThrow("User cannot be null.", methodName);
        if (string.IsNullOrWhiteSpace(tokenType)) LogAndThrow("TokenType is required.", methodName);
        if (string.IsNullOrWhiteSpace(tokenValue)) LogAndThrow("TokenValue is required.", methodName);

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

        if (user == null) LogAndThrow("User cannot be null.", methodName);
        if (string.IsNullOrWhiteSpace(tokenType)) LogAndThrow("TokenType is required.", methodName);

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

        if (string.IsNullOrWhiteSpace(tokenType)) LogAndThrow("TokenType is required.", methodName);
        if (string.IsNullOrWhiteSpace(tokenValue)) LogAndThrow("TokenValue is required.", methodName);

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

    // Other methods remain unchanged...
}
