using Bonyan.DependencyInjection;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions.Repositories;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.UnitOfWork;
using Bonyan.Workers;
using Microsoft.Extensions.Hosting;

namespace Bonyan.IdentityManagement.Application.Permissions.Workers
{
    public class BonIdentityPermissionSeeder : BackgroundService
    {
        private readonly IBonUnitOfWorkManager _unitOfWorkManager;
        private readonly IBonPermissionManager _permissionManager;
        private readonly IBonIdentityPermissionRepository _bonIdentityPermissionRepository;

        public BonIdentityPermissionSeeder(IBonUnitOfWorkManager unitOfWorkManager,
            IBonIdentityPermissionRepository bonIdentityPermissionRepository, IBonPermissionManager permissionManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _bonIdentityPermissionRepository = bonIdentityPermissionRepository;
            _permissionManager = permissionManager;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _unitOfWorkManager.Begin();

            // Fetch all permissions in the database
            var existingPermissions = await _bonIdentityPermissionRepository.FindAsync(_ => true);

            // Get the keys of the permissions stored in BonPermissionAccessor
            var permissionKeys = _permissionManager.GetAllPermissions().ToHashSet();

            // Identify and remove permissions from DB that are not in the BonPermissionAccessor
            foreach (var dbPermission in existingPermissions)
            {
                if (!permissionKeys.Contains(dbPermission))
                {
                    // If the permission is not in BonPermissionAccessor, delete it
                    await _bonIdentityPermissionRepository.DeleteAsync(dbPermission, true);
                }
            }

            // Add any permissions from BonPermissionAccessor that do not exist in the DB
            foreach (var permission in permissionKeys)
            {
                if (!await _bonIdentityPermissionRepository.ExistsAsync(x => x.Id.Equals(permission.Id)))
                {
                    // If the permission does not exist, add it
                    await _bonIdentityPermissionRepository.AddAsync(
                        permission, true);
                }
            }

            // Commit the changes
            await uow.CompleteAsync();
        }
    }
}