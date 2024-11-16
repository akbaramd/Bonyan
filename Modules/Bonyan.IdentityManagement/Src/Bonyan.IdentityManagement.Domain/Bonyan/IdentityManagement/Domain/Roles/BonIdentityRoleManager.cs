using Bonyan.IdentityManagement.Domain.Abstractions.Roles;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.Layer.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Domain.Roles
{
    public class BonIdentityRoleManager : BonDomainService, IBonIdentityRoleManager
    {
        public IBonRoleRepository RoleRepository => LazyServiceProvider.LazyGetRequiredService<IBonRoleRepository>();

        // Create a new role
        public async Task<BonDomainResult> CreateAsync(string name, string title)
        {
            try
            {
                if (await RoleRepository.ExistsAsync(x => x.Name.Equals(name)))
                {
                    Logger.LogWarning($"Role with name {name} already exists.");
                    return BonDomainResult.Failure($"Role with name {name} already exists.");
                }

                var role = new BonIdentityRole(BonRoleId.CreateNew(), name, title);
                await RoleRepository.AddAsync(role, true);
                return BonDomainResult.Success();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error creating role.");
                return BonDomainResult.Failure("Error creating role.");
            }
        }

        // Update role details
        public async Task<BonDomainResult> UpdateAsync(BonIdentityRole entity)
        {
            try
            {
                await RoleRepository.UpdateAsync(entity, true);
                return BonDomainResult.Success();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error updating role.");
                return BonDomainResult.Failure("Error updating role.");
            }
        }

        // Find a role by its name
        public async Task<BonDomainResult<BonIdentityRole>> FindByNameAsync(string name)
        {
            try
            {
                var role = await RoleRepository.FindOneAsync(x => x.Name.Equals(name));
                if (role == null)
                {
                    return BonDomainResult<BonIdentityRole>.Failure($"Role with name {name} not found.");
                }
                return BonDomainResult<BonIdentityRole>.Success(role);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error finding role by name.");
                return BonDomainResult<BonIdentityRole>.Failure("Error finding role by name.");
            }
        }

        // Delete a role and optionally remove it from all users
        public async Task<BonDomainResult> DeleteAsync(BonIdentityRole identityRole)
        {
            try
            {
                await RoleRepository.DeleteAsync(identityRole, true);
                return BonDomainResult.Success();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error deleting identityRole.");
                return BonDomainResult.Failure("Error deleting role.");
            }
        }

        // Check if a role exists by name
        public async Task<BonDomainResult<bool>> RoleExistsAsync(string roleName)
        {
            try
            {
                var exists = await RoleRepository.ExistsAsync(r => r.Name == roleName);
                return BonDomainResult<bool>.Success(exists);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error checking if role exists.");
                return BonDomainResult<bool>.Failure("Error checking if role exists.");
            }
        }
    }
}
