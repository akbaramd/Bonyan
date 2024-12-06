using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions.Repositories;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Application.Permissions;

public class BonIdentityPermissionAppService :
    BonApplicationService,
    IBonIdentityPermissionAppService
{

    IBonIdentityPermissionReadOnlyRepository PermissionReadOnlyRepository=> LazyServiceProvider.LazyGetRequiredService<IBonIdentityPermissionReadOnlyRepository>();

    public async Task<ServiceResult<BonIdentityPermissionDto>> DetailAsync(BonPermissionId key)
    {
        try
        {
            if (key == null)
                return ServiceResult<BonIdentityPermissionDto>.Failure("Invalid permission id");

            var permission = await PermissionReadOnlyRepository.FindByIdAsync(key);

            if (permission == null)
                return ServiceResult<BonIdentityPermissionDto>.Failure("Permission not found");

            var result = Mapper.Map<BonIdentityPermissionDto>(permission);

            if (result == null)
                return ServiceResult<BonIdentityPermissionDto>.Failure("Mapping failed");

            return ServiceResult<BonIdentityPermissionDto>.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<BonIdentityPermissionDto>.Failure($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BonPaginatedResult<BonIdentityPermissionDto>>> PaginatedAsync(BonFilterAndPaginateDto paginateDto)
    {
        try
        {
            if (paginateDto == null)
                return ServiceResult<BonPaginatedResult<BonIdentityPermissionDto>>.Failure("Invalid pagination parameters");

            if (paginateDto.Take <= 0 || paginateDto.Skip < 0)
                return ServiceResult<BonPaginatedResult<BonIdentityPermissionDto>>.Failure("Invalid take or skip values");

            var permission = await PermissionReadOnlyRepository.PaginatedAsync(
                x => string.IsNullOrEmpty(paginateDto.Search) || x.Title.Contains(paginateDto.Search),
                paginateDto.Take,
                paginateDto.Skip);

            if (permission == null)
                return ServiceResult<BonPaginatedResult<BonIdentityPermissionDto>>.Failure("No data found");

           var mappedRoles = Mapper.Map<IEnumerable<BonIdentityPermissionDto>>(permission.Results);

                var result = new BonPaginatedResult<BonIdentityPermissionDto>(
                    mappedRoles,
                    paginateDto.Skip,
                    paginateDto.Take,
                    permission.TotalCount
                );

            return ServiceResult<BonPaginatedResult<BonIdentityPermissionDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<BonPaginatedResult<BonIdentityPermissionDto>>.Failure($"An error occurred: {ex.Message}");
        }
    }
}