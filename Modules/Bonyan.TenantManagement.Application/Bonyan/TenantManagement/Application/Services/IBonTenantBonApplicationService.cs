using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Abstractions.Results;
using Bonyan.TenantManagement.Application.Dto;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.Application.Services;

public interface IBonTenantBonApplicationService : IBonApplicationService
{
  Task<BonPaginatedResult<BonTenantDto>> PaginateAsync(BonTenantFilterDto paginateDto,
    CancellationToken? cancellationToken = default);

  Task<BonTenantDto> DetailAsync(BonTenantId bonTenantId, CancellationToken? cancellationToken = default);
  Task<BonTenantDto> CreateAsync(BonTenantCreateDto createDto, CancellationToken? cancellationToken = default);

  Task<BonTenantDto> UpdateAsync(BonTenantId bonTenantId, BonTenantUpdateDto updateDto,
    CancellationToken? cancellationToken = default);

  Task<BonTenantDto> DeleteAsync(BonTenantId bonTenantId, CancellationToken? cancellationToken = default);
}
