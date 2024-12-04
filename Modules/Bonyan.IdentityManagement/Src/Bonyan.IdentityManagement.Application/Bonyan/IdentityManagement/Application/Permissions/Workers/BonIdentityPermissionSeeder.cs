using Bonyan.AspNetCore.Authorization.Permissions;
using Bonyan.DependencyInjection;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions.Repositories;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.UnitOfWork;
using Bonyan.Workers;

namespace Bonyan.IdentityManagement.Application.Permissions.Workers
{
    public class BonIdentityPermissionSeeder : IBonWorker
    {
        private readonly IBonUnitOfWorkManager _unitOfWorkManager;
        private readonly PermissionAccessor _permissionAccessor;
        private readonly IBonIdentityPermissionRepository _bonIdentityPermissionRepository;

        public BonIdentityPermissionSeeder(IBonUnitOfWorkManager unitOfWorkManager,
            IBonObjectAccessor<PermissionAccessor> permissionAccessor,
            IBonIdentityPermissionRepository bonIdentityPermissionRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _permissionAccessor = permissionAccessor.Value;
            _bonIdentityPermissionRepository = bonIdentityPermissionRepository;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            using var uow = _unitOfWorkManager.Begin();

            // Fetch all permissions in the database
            var existingPermissions = await _bonIdentityPermissionRepository.FindAsync(_ => true);

            // Get the keys of the permissions stored in PermissionAccessor
            var permissionKeys = _permissionAccessor.ToHashSet();

            // Identify and remove permissions from DB that are not in the PermissionAccessor
            foreach (var dbPermission in existingPermissions)
            {
                if (!permissionKeys.Contains(dbPermission.Id.Value))
                {
                    // If the permission is not in PermissionAccessor, delete it
                    await _bonIdentityPermissionRepository.DeleteAsync(dbPermission, true);
                }
            }

            // Add any permissions from PermissionAccessor that do not exist in the DB
            foreach (var permission in permissionKeys)
            {
                if (!await _bonIdentityPermissionRepository.ExistsAsync(x => x.Id.Equals(BonPermissionId.NewId(permission))))
                {
                    // If the permission does not exist, add it
                    await _bonIdentityPermissionRepository.AddAsync(
                        new BonIdentityPermission(BonPermissionId.NewId(permission), permission), true);
                }
            }

            // Commit the changes
            await uow.CompleteAsync();
        }
    }
}