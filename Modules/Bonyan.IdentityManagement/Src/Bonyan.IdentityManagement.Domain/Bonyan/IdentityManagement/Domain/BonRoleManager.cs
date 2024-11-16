using Bonyan.Layer.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Domain
{
    public class BonRoleManager : BonDomainService
    {
        public IBonRoleRepository RoleRepository => LazyServiceProvider.LazyGetRequiredService<IBonRoleRepository>();

        // Create a new role
        public async Task<bool> CreateAsync(string name, string title)
        {
            try
            {
                if (await RoleRepository.ExistsAsync(x => x.Name.Equals(name)))
                {
                    Logger.LogWarning($"Role with name {name} already exists.");
                    return false;
                }

                var role = new BonIdentityRole(BonRoleId.CreateNew(), name, title);
                await RoleRepository.AddAsync(role, true);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error creating role.");
                return false;
            }
        }

        // Update role details
        public async Task<bool> UpdateAsync(BonIdentityRole Entity)
        {
            try
            {
                await RoleRepository.UpdateAsync(Entity, true);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error updating role.");
                return false;
            }
        }

        // Find a role by its name
        public async Task<BonIdentityRole?> FindByNameAsync(string name)
        {
            try
            {
                return await RoleRepository.FindOneAsync(x => x.Name.Equals(name));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error finding role by name.");
                return null;
            }
        }

        // Delete a role and optionally remove it from all users
        public async Task<bool> DeleteAsync(BonIdentityRole role)
        {
            try
            {
              

                await RoleRepository.DeleteAsync(role, true);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error deleting role.");
                return false;
            }
        }

      
        // Check if a role exists by name
        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await RoleRepository.ExistsAsync(r => r.Name == roleName);
        }
    }
}
