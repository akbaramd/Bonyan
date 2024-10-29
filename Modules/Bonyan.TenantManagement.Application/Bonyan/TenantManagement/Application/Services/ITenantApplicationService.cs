using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Model;
using Bonyan.TenantManagement.Application.Dto;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.Application.Services;

public interface ITenantApplicationService : IApplicationService
{
  Task<PaginatedResult<TenantDto>> PaginateAsync(TenantFilterDto paginateDto,
    CancellationToken? cancellationToken = default);

  Task<TenantDto> DetailAsync(TenantId tenantId, CancellationToken? cancellationToken = default);
  Task<TenantDto> CreateAsync(TenantCreateDto createDto, CancellationToken? cancellationToken = default);

  Task<TenantDto> UpdateAsync(TenantId tenantId, TenantUpdateDto updateDto,
    CancellationToken? cancellationToken = default);

  Task<TenantDto> DeleteAsync(TenantId tenantId, CancellationToken? cancellationToken = default);
}
