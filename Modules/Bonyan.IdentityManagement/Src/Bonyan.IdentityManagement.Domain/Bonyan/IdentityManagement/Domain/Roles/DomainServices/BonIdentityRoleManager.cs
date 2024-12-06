using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions.Repositories;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.DomainService;

namespace Bonyan.IdentityManagement.Domain.Roles.DomainServices
{
    public class BonIdentityRoleManager : IBonIdentityRoleManager
    {
        private readonly IBonIdentityRoleRepository _roleRepository;
        private readonly IBonIdentityPermissionRepository _permissionRepository;

        public BonIdentityRoleManager(
            IBonIdentityRoleRepository roleRepository,
            IBonIdentityPermissionRepository permissionRepository)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }

        /// <summary>
        /// Creates a role.
        /// </summary>
        public async Task<BonDomainResult> CreateRoleAsync(BonIdentityRole role)
        {
            if (await _roleRepository.ExistsAsync(x => x.Id == role.Id))
            {
                return BonDomainResult.Failure("Role already exists.");
            }

            await _roleRepository.AddAsync(role, true);
            return BonDomainResult.Success();
        }

        /// <summary>
        /// Updates an existing role.
        /// </summary>
        public async Task<BonDomainResult> UpdateRoleAsync(BonIdentityRole role)
        {
            if (!await _roleRepository.ExistsAsync(x => x.Id == role.Id))
            {
                return BonDomainResult.Failure("Role does not exist.");
            }

            await _roleRepository.UpdateAsync(role, true);
            return BonDomainResult.Success();
        }

        /// <summary>
        /// Deletes a role.
        /// </summary>
        public async Task<BonDomainResult> DeleteRoleAsync(BonIdentityRole role)
        {
            if (!await _roleRepository.ExistsAsync(x => x.Id == role.Id))
            {
                return BonDomainResult.Failure("Role does not exist.");
            }

            role.Delete(); // Domain behavior ensures the role can be deleted
            await _roleRepository.DeleteAsync(role, true);

            return BonDomainResult.Success();
        }

        /// <summary>
        /// Creates a role with associated permissions.
        /// </summary>
        public async Task<BonDomainResult> CreateRoleWithPermissionsAsync(BonIdentityRole role, IEnumerable<BonPermissionId> permissionIds)
        {
            var createRoleResult = await CreateRoleAsync(role);
            if (!createRoleResult.IsSuccess)
            {
                return createRoleResult;
            }

            var assignResult = await AssignPermissionsToRoleAsync(role, permissionIds);
            return assignResult.IsSuccess
                ? BonDomainResult.Success()
                : BonDomainResult.Failure(assignResult.ErrorMessage);
        }

        /// <summary>
        /// Assigns permissions to a role using the role's domain behavior.
        /// </summary>
        public async Task<BonDomainResult> AssignPermissionsToRoleAsync(BonIdentityRole role, IEnumerable<BonPermissionId> permissionIds)
        {
            try
            {
                // Retrieve valid permissions
                var permissionsResult = await GetPermissionsAsync(permissionIds);
                if (!permissionsResult.IsSuccess)
                {
                    return BonDomainResult.Failure(permissionsResult.ErrorMessage);
                }

                var permissions = permissionsResult.Value;

                // Use domain behavior to assign permissions
                foreach (var permission in permissions)
                {
                    role.AssignPermission(permission.Id);
                }

                // Persist changes
                await _roleRepository.UpdateAsync(role, true);
                return BonDomainResult.Success();
            }
            catch (Exception ex)
            {
                return BonDomainResult.Failure($"Error assigning permissions: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a permission from a role.
        /// </summary>
        public async Task<BonDomainResult> RemovePermissionFromRoleAsync(BonIdentityRole role, BonPermissionId permissionId)
        {
            try
            {
                role.RemovePermission(permissionId);

                // Persist changes
                await _roleRepository.UpdateAsync(role, true);
                return BonDomainResult.Success();
            }
            catch (Exception ex)
            {
                return BonDomainResult.Failure($"Error removing permission: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves permissions from the database.
        /// </summary>
        private async Task<BonDomainResult<List<BonIdentityPermission>>> GetPermissionsAsync(IEnumerable<BonPermissionId> permissionIds)
        {
            var permissions = await _permissionRepository.FindAsync(p => permissionIds.Contains(p.Id));
            var missingPermissions = permissionIds.Except(permissions.Select(p => p.Id)).ToList();

            if (missingPermissions.Any())
            {
                var missingIds = string.Join(", ", missingPermissions.Select(p => p.Value));
                return BonDomainResult<List<BonIdentityPermission>>.Failure($"Permissions not found: {missingIds}");
            }

            return BonDomainResult<List<BonIdentityPermission>>.Success(permissions.ToList());
        }

        /// <summary>
        /// Finds a role by its ID.
        /// </summary>
        public async Task<BonDomainResult<BonIdentityRole>> FindRoleByIdAsync(string roleKey)
        {
            var role = await _roleRepository.FindOneAsync(x => x.Id == BonRoleId.NewId(roleKey));
            if (role == null)
            {
                return BonDomainResult<BonIdentityRole>.Failure("Role not found.");
            }

            return BonDomainResult<BonIdentityRole>.Success(role);
        }
    }
}
