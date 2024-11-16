using Bonyan.Layer.Domain.Services;
using Bonyan.UserManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Domain
{
    public class BonUserManager<TUser> : BonDomainService where TUser : BonIdentityUser
    {
        public IBonRoleRepository RoleRepository => LazyServiceProvider.LazyGetRequiredService<IBonRoleRepository>();

        public IBonUserRepository<TUser> UserRepository =>
            LazyServiceProvider.LazyGetRequiredService<IBonUserRepository<TUser>>();

        // Create a new user and set an initial password
        public async Task<bool> CreateAsync(TUser Entity, string password)
        {
            try
            {
                if (await UserRepository.ExistsAsync(x => x.UserName.Equals(Entity.UserName)))
                {
                    Logger.LogWarning($"User with username {Entity.UserName} already exists.");
                    return false;
                }

                Entity.SetPassword(password);
                await UserRepository.AddAsync(Entity, true);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error creating user.");
                return false;
            }
        }

        // Update user information
        public async Task<bool> UpdateAsync(TUser Entity)
        {
            try
            {
                await UserRepository.UpdateAsync(Entity, true);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error updating user.");
                return false;
            }
        }

        // Find user by username
        public async Task<TUser?> FindByUserNameAsync(string userName)
        {
            try
            {
                return await UserRepository.FindOneAsync(x => x.UserName.Equals(userName));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error finding user by username.");
                return null;
            }
        }

        // Change a user's password
        public async Task<bool> ChangePasswordAsync(TUser Entity, string currentPassword, string newPassword)
        {
            if (!Entity.VerifyPassword(currentPassword))
            {
                Logger.LogWarning("Current password does not match.");
                return false;
            }

            Entity.SetPassword(newPassword);
            return await UpdateAsync(Entity);
        }

        // Reset a user's password directly (for admin use cases)
        public async Task<bool> ResetPasswordAsync(TUser Entity, string newPassword)
        {
            Entity.SetPassword(newPassword);
            return await UpdateAsync(Entity);
        }

        // Assign multiple roles to a user
        public async Task<bool> AssignRolesAsync(TUser Entity, params string[] roles)
        {
            try
            {
                foreach (var role in roles)
                {
                    var findRole = await RoleRepository.FindOneAsync(x => x.Name.Equals(role));
                    if (findRole != null)
                    {
                        Entity.TryAssignRole(findRole);
                    }
                }

                await UserRepository.UpdateAsync(Entity, true);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error assigning roles to user.");
                return false;
            }
        }

        // Remove multiple roles from a user
        public async Task<bool> RemoveRolesAsync(TUser Entity, params string[] roles)
        {
            try
            {
                foreach (var roleName in roles)
                {
                    var role = await RoleRepository.FindOneAsync(x => x.Name.Equals(roleName));
                    if (role != null)
                    {
                        Entity.TryRemoveRole(role);
                    }
                }

                await UserRepository.UpdateAsync(Entity, true);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error removing roles from user.");
                return false;
            }
        }

        // Update roles assigned to a user, syncing with the provided list of roles
        public async Task UpdateRolesAsync(TUser user, string[] roles)
        {
            try
            {
                var currentRoles = user.Roles.Select(r => r.Name).ToHashSet();
                var rolesToRemove = currentRoles.Except(roles).ToArray();
                var rolesToAdd = roles.Except(currentRoles).ToArray();

                // Remove roles not in the new list
                foreach (var roleName in rolesToRemove)
                {
                    var role = await RoleRepository.FindOneAsync(x => x.Name.Equals(roleName));
                    if (role != null)
                    {
                        user.TryRemoveRole(role);
                    }
                }

                // Add roles that are in the new list but not currently assigned
                foreach (var roleName in rolesToAdd)
                {
                    var role = await RoleRepository.FindOneAsync(x => x.Name.Equals(roleName));
                    if (role != null)
                    {
                        user.TryAssignRole(role);
                    }
                }

                await UserRepository.UpdateAsync(user, true);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error updating roles for user.");
                throw new Exception("An error occurred while updating roles.", e);
            }
        }

        // Check if a user has a specific role
        public bool HasRole(TUser user, string roleName)
        {
            return user.Roles.Any(role => role.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        }
    }
}