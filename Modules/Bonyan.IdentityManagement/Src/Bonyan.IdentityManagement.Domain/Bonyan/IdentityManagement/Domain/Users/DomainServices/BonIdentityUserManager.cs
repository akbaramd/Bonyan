using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.UserManagement.Domain.Users.DomainServices;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Domain.Users.DomainServices;

public class BonIdentityUserManager<TUser> : BonUserManager<TUser>, IBonIdentityUserManager<TUser>
    where TUser : class, IBonIdentityUser
{
    public IBonIdentityRoleRepository RoleRepository =>
        LazyServiceProvider.LazyGetRequiredService<IBonIdentityRoleRepository>();

    public IBonIdentityUserRolesRepository UserRolesRepository =>
        LazyServiceProvider.LazyGetRequiredService<IBonIdentityUserRolesRepository>();

    // Assign role to the user
    public async Task<BonDomainResult> AssignRoleAsync(TUser user, string roleName)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrEmpty(roleName)) throw new ArgumentNullException(nameof(roleName));

        try
        {
            // Get the role object by name
            var role = await RoleRepository.FindOneAsync(x => x.Id.Value == roleName);
            if (role == null)
            {
                return BonDomainResult.Failure($"Role {roleName} does not exist.");
            }

            // Check if the role already exists for the user to avoid duplicate entries
            var userRole = await UserRolesRepository.FindOneAsync(x => x.UserId == user.Id && x.RoleId == role.Id);
            if (userRole != null)
            {
                return BonDomainResult.Failure($"User already has the role {roleName}.");
            }

            // Add the role to the UserRoles table
            userRole = new BonIdentityUserRoles(user.Id, role.Id);
            await UserRolesRepository.AddAsync(userRole, true);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error assigning role to user.");
            return BonDomainResult.Failure("Error assigning role to user.");
        }
    }

    // Remove role from the user
    public async Task<BonDomainResult> RemoveRoleAsync(TUser user, string roleName)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrEmpty(roleName)) throw new ArgumentNullException(nameof(roleName));

        try
        {
            // Get the role object by name
            var role = await RoleRepository.FindOneAsync(x => x.Id.Value == roleName);
            if (role == null)
            {
                return BonDomainResult.Failure($"Role {roleName} does not exist.");
            }

            // Get the user-role association and remove it
            var userRoles = await UserRolesRepository.FindAsync(x => x.UserId == user.Id && x.RoleId == role.Id);
            var userRole = userRoles.FirstOrDefault();
            if (userRole == null)
            {
                return BonDomainResult.Failure($"User does not have the role {roleName}.");
            }

            await UserRolesRepository.DeleteAsync(userRole, true);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error removing role from user.");
            return BonDomainResult.Failure("Error removing role from user.");
        }
    }

    // Get all roles assigned to the user
    public async Task<BonDomainResult<IReadOnlyList<string>>> GetUserRolesAsync(TUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        try
        {
            // Fetch all role IDs for the user in a single query
            var userRoles = await UserRolesRepository.FindAsync(x => x.UserId == user.Id);
            if (userRoles == null || !userRoles.Any())
            {
                return BonDomainResult<IReadOnlyList<string>>.Success(new List<string>());
            }

            // Extract role IDs from the user-role associations and fetch roles in a batch query
            var roleIds = userRoles.Select(x => x.RoleId).ToList();
            var roles = await RoleRepository.FindAsync(x => roleIds.Contains(x.Id));

            return BonDomainResult<IReadOnlyList<string>>.Success(roles.Select(r => r.Id.Value).ToList());
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error fetching user roles.");
            return BonDomainResult<IReadOnlyList<string>>.Failure("Error fetching user roles.");
        }
    }

    // Create a new user and set password
    public async Task<BonDomainResult> CreateAsync(TUser entity, string password)
    {
        try
        {
            entity.SetPassword(password);
            return await CreateAsync(entity);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error creating user.");
            return BonDomainResult.Failure(e.Message);
        }
    }

    // Change a user's password
    public async Task<BonDomainResult> ChangePasswordAsync(TUser entity, string currentPassword, string newPassword)
    {
        if (!entity.VerifyPassword(currentPassword))
        {
            Logger.LogWarning("Current password does not match.");
            return BonDomainResult.Failure("Current password does not match.");
        }

        entity.SetPassword(newPassword);
        return await UpdateAsync(entity);
    }

    // Reset a user's password directly (for admin use cases)
    public async Task<BonDomainResult> ResetPasswordAsync(TUser entity, string newPassword)
    {
        entity.SetPassword(newPassword);
        return await UpdateAsync(entity);
    }
    
    // Add a token to the user
    public async Task<BonDomainResult> SetTokenAsync(TUser user, string tokenType, string tokenValue, DateTime? expiration = null)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(tokenType)) throw new ArgumentException("Token type is required.", nameof(tokenType));
        if (string.IsNullOrWhiteSpace(tokenValue)) throw new ArgumentException("Token value is required.", nameof(tokenValue));

        try
        {
            user.SetToken(tokenType, tokenValue, expiration);
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error adding token to user.");
            return BonDomainResult.Failure("Error adding token to user.");
        }
    }

    // Remove a token from the user
    public async Task<BonDomainResult> RemoveTokenAsync(TUser user, string tokenType)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(tokenType)) throw new ArgumentException("Token type is required.", nameof(tokenType));

        try
        {
            user.RemoveToken(tokenType);
            await UpdateAsync(user);
            return BonDomainResult.Success();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error removing token from user.");
            return BonDomainResult.Failure("Error removing token from user.");
        }
    }

    // Update a token for the user


    // Find a user by token
    public async Task<BonDomainResult<TUser>> FindByTokenAsync(string tokenType, string tokenValue)
    {
        if (string.IsNullOrWhiteSpace(tokenType)) throw new ArgumentException("Token type is required.", nameof(tokenType));
        if (string.IsNullOrWhiteSpace(tokenValue)) throw new ArgumentException("Token value is required.", nameof(tokenValue));

        try
        {
            // Search directly through user repository by checking token
            var user = await UserRepository.FindOneAsync(u => u.Tokens.Any(t => t.Type == tokenType && t.Value == tokenValue));
            if (user == null)
            {
                return BonDomainResult<TUser>.Failure($"No user found with the specified token.");
            }

            return BonDomainResult<TUser>.Success(user);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error finding user by token.");
            return BonDomainResult<TUser>.Failure("Error finding user by token.");
        }
    }
}