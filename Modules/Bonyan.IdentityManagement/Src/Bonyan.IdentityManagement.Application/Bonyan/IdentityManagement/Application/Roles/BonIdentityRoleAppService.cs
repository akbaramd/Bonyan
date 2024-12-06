using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bonyan.IdentityManagement.Application.Roles
{
    public class BonIdentityRoleAppService : BonApplicationService, IBonIdentityRoleAppService
    {
        private readonly IBonIdentityRoleRepository _roleRepository;

        public BonIdentityRoleAppService(IBonIdentityRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<ServiceResult<BonIdentityRoleDto>> CreateAsync(BonIdentityRoleCreateDto input)
        {
            try
            {
                if (input == null)
                    return ServiceResult<BonIdentityRoleDto>.Failure("Invalid input");

                var role = new BonIdentityRole(BonRoleId.NewId(input.Key), input.Title);

                foreach (var permission in input.Permissions)
                {
                    role.AssignPermission(BonPermissionId.NewId(permission));
                }

                await _roleRepository.AddAsync(role,true);
                return await DetailAsync(role.Id);
            }
            catch (Exception ex)
            {
                return ServiceResult<BonIdentityRoleDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteAsync(BonRoleId id)
        {
            try
            {
                if (id == null)
                    return ServiceResult.Failure("Invalid role id");

                var role = await _roleRepository.FindOneAsync(x => x.Id == id);
                if (role == null)
                    return ServiceResult.Failure("Role not found");

                await _roleRepository.DeleteAsync(role);

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BonIdentityRoleDto>> DetailAsync(BonRoleId key)
        {
            try
            {
                if (key == null)
                    return ServiceResult<BonIdentityRoleDto>.Failure("Invalid role id");

                var role = await _roleRepository.FindOneAsync(x => x.Id == key);
                if (role == null)
                    return ServiceResult<BonIdentityRoleDto>.Failure("Role not found");

                var roleDto = Mapper.Map<BonIdentityRoleDto>(role);
                return ServiceResult<BonIdentityRoleDto>.Success(roleDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<BonIdentityRoleDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BonPaginatedResult<BonIdentityRoleDto>>> PaginatedAsync(BonFilterAndPaginateDto paginateDto)
        {
            try
            {
                if (paginateDto == null)
                    return ServiceResult<BonPaginatedResult<BonIdentityRoleDto>>.Failure("Invalid pagination parameters");

                if (paginateDto.Take <= 0 || paginateDto.Skip < 0)
                    return ServiceResult<BonPaginatedResult<BonIdentityRoleDto>>.Failure("Invalid take or skip values");

                var roles = await _roleRepository.PaginatedAsync(
                    x => string.IsNullOrEmpty(paginateDto.Search) || x.Title.Contains(paginateDto.Search),
                    paginateDto.Take,
                    paginateDto.Skip);

                if (roles == null)
                    return ServiceResult<BonPaginatedResult<BonIdentityRoleDto>>.Failure("No data found");

                var mappedRoles = Mapper.Map<IEnumerable<BonIdentityRoleDto>>(roles.Results);

                var result = new BonPaginatedResult<BonIdentityRoleDto>(
                    mappedRoles,
                    paginateDto.Skip,
                    paginateDto.Take,
                    roles.TotalCount
                );

                return ServiceResult<BonPaginatedResult<BonIdentityRoleDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<BonPaginatedResult<BonIdentityRoleDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BonIdentityRoleDto>> UpdateAsync(BonRoleId key, BonIdentityRoleUpdateDto input)
        {
            try
            {
                if (key == null)
                    return ServiceResult<BonIdentityRoleDto>.Failure("Invalid role id");

                if (input == null || string.IsNullOrWhiteSpace(input.Title))
                    return ServiceResult<BonIdentityRoleDto>.Failure("Invalid input");

                var role = await _roleRepository.FindOneAsync(x => x.Id == key);
                if (role == null)
                    return ServiceResult<BonIdentityRoleDto>.Failure("Role not found");

                // Update title
                role.SetTitle(input.Title);

                // Handle permissions only if input.Permissions is not null
                if (input.Permissions != null)
                {
                    // Remove permissions that are not in input
                    var permissionsToRemove = role.RolePermissions
                        .Where(rp => !input.Permissions.Contains(rp.Permission.Id.Value))
                        .ToList();

                    foreach (var rp in permissionsToRemove)
                    {
                        role.RemovePermission(rp.Permission.Id);
                    }

                    // Add new permissions from input that don't exist
                    var existingPermissionIds = role.RolePermissions
                        .Select(rp => rp.Permission.Id.Value)
                        .ToList();

                    var newPermissions = input.Permissions
                        .Where(p => !existingPermissionIds.Contains(p))
                        .ToList();

                    foreach (var newPermission in newPermissions)
                    {
                        role.AssignPermission(BonPermissionId.FromValue(newPermission));
                    }
                }

                await _roleRepository.UpdateAsync(role, true);

                return await DetailAsync(role.Id);
            }
            catch (Exception ex)
            {
                return ServiceResult<BonIdentityRoleDto>.Failure($"An error occurred while updating role: {ex.Message}");
            }
        }
    }
}