using Bonyan.IdentityManagement.Domain.Permissions.Repositories;
using Bonyan.IdentityManagement.Domain.Permissions;
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
        private readonly IBonIdentityRolePermissionRepository _rolePermissionRepository;

        public BonIdentityRoleManager(
            IBonIdentityRoleRepository roleRepository,
            IBonIdentityPermissionRepository permissionRepository, 
            IBonIdentityRolePermissionRepository rolePermissionRepository)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task<BonDomainResult> CreateRoleAsync(BonIdentityRole role)
        {
            if (await _roleRepository.ExistsAsync(x => x.Id == role.Id))
            {
                return BonDomainResult.Failure("Role already exists.");
            }

            await _roleRepository.AddAsync(role, true);

            return BonDomainResult.Success();
        }

        public async Task<BonDomainResult> CreateRoleWithPermissionsAsync(BonIdentityRole role, IEnumerable<BonPermissionId> permissionIds)
        {
            // Initialize error accumulator
            var errorMessages = new List<string>();

            // Step 1: Create the role
            var createResult = await CreateRoleAsync(role);
            if (!createResult.IsSuccess)
            {
                errorMessages.Add(createResult.ErrorMessage);
            }

            // Step 2: Assign permissions to the role
            var permissionResult = await AssignPermissionsToRoleAsync(role, permissionIds);
            if (!permissionResult.IsSuccess)
            {
                errorMessages.Add(permissionResult.ErrorMessage);
            }

            // If there were any errors, return a combined failure
            if (errorMessages.Any())
            {
                return BonDomainResult.Failure(string.Join(", ", errorMessages));
            }

            // Everything succeeded, return success
            return BonDomainResult.Success();
        }

        public async Task<BonDomainResult> UpdateRoleAsync(BonIdentityRole role)
        {
            if (!await _roleRepository.ExistsAsync(x => x.Id == role.Id))
            {
                return BonDomainResult.Failure("Role does not exist.");
            }

            await _roleRepository.UpdateAsync(role, true);

            return BonDomainResult.Success();
        }

        public async Task<BonDomainResult> DeleteRoleAsync(BonIdentityRole role)
        {
            if (!await _roleRepository.ExistsAsync(x => x.Id == role.Id))
            {
                return BonDomainResult.Failure("Role does not exist.");
            }

            await _roleRepository.DeleteAsync(role, true);

            return BonDomainResult.Success();
        }

        public async Task<BonDomainResult> AssignPermissionsToRoleAsync(BonIdentityRole role, IEnumerable<BonPermissionId> permissionIds)
        {
            var permissionsResult = await GetPermissionsAsync(permissionIds);
            if (!permissionsResult.IsSuccess)
            {
                return BonDomainResult.Failure(permissionsResult.ErrorMessage);
            }

            var permissions = permissionsResult.Value;
            // Fetch current role permissions from the database
            var currentPermissions = await _rolePermissionRepository.FindAsync(x => x.RoleId == role.Id);

            // Find permissions to add (permissions in the input that are not yet assigned)
            var permissionsToAdd = permissions
                .Where(p => !currentPermissions.Any(cp => cp.PermissionId == p.Id))
                .ToList();

            // Find permissions to remove (permissions currently assigned but not in the input)
            var permissionsToRemove = currentPermissions
                .Where(cp => !permissions.Any(p => p.Id == cp.PermissionId))
                .ToList();

            // Remove the old permissions from the role
            await _rolePermissionRepository.DeleteRangeAsync(permissionsToRemove);

            // Add the new permissions to the role
            foreach (var permissionToAdd in permissionsToAdd)
            {
                await _rolePermissionRepository.AddAsync(new BonIdentityRolePermissions(role.Id, permissionToAdd.Id));
            }

            return BonDomainResult.Success();
        }

        public async Task<BonDomainResult<BonIdentityRole>> FindRoleByKeyAsync(string roleKey)
        {
            var role = await _roleRepository.FindOneAsync(x => x.Id == BonRoleId.NewId(roleKey));
            if (role == null)
            {
                return BonDomainResult<BonIdentityRole>.Failure("Role not found.");
            }

            return BonDomainResult<BonIdentityRole>.Success(role);
        }

        private async Task<BonDomainResult<List<BonIdentityPermission>>> GetPermissionsAsync(IEnumerable<BonPermissionId> permissionIds)
        {
            var permissions = new List<BonIdentityPermission>();
            var missingPermissions = new List<BonPermissionId>();

            foreach (var permissionId in permissionIds)
            {
                var permission = await _permissionRepository.FindOneAsync(x => x.Id == permissionId);
                if (permission == null)
                {
                    missingPermissions.Add(permissionId);
                }
                else
                {
                    permissions.Add(permission);
                }
            }

            if (missingPermissions.Any())
            {
                var missingIds = string.Join(", ", missingPermissions.Select(p => p.Value));
                return BonDomainResult<List<BonIdentityPermission>>.Failure($"Permissions not found: {missingIds}");
            }

            return BonDomainResult<List<BonIdentityPermission>>.Success(permissions);
        }
    }
}
