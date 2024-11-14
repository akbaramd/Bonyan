using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.TenantManagement.Application.Dto;
using Bonyan.TenantManagement.Application.Exceptions;
using Bonyan.TenantManagement.Application.Specs;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.Application.Services;

public class BonTenantBonApplicationService : BonApplicationService, IBonTenantBonApplicationService
{
  public IBonTenantRepository BonTenantRepository => LazyServiceProvider.LazyGetRequiredService<IBonTenantRepository>();

  public async Task<BonPaginatedResult<BonTenantDto>> PaginateAsync(BonTenantFilterDto paginateDto, CancellationToken? cancellationToken = default)
  {
    var paginated = await BonTenantRepository.PaginatedAsync(new TenantPaginateSpec(paginateDto));
    return Mapper.Map<BonPaginatedResult<BonTenant>, BonPaginatedResult<BonTenantDto>>(paginated);
  }

  public async Task<BonTenantDto> DetailAsync(BonTenantId bonTenantId, CancellationToken? cancellationToken = default)
  {
    var tenant = await BonTenantRepository.FindByIdAsync(bonTenantId);
    if (tenant == null) throw new BonTenantNotFoundException(tenantId:bonTenantId,errorCode:$"{nameof(BonTenantBonApplicationService)}:{nameof(DetailAsync)}");

    return Mapper.Map<BonTenant, BonTenantDto>(tenant);
  }

  public async Task<BonTenantDto> CreateAsync(BonTenantCreateDto createDto, CancellationToken? cancellationToken = default)
  {
    var tenant = new BonTenant(createDto.Key);
    await BonTenantRepository.AddAsync(tenant);
    await CurrentUnitOfWork.SaveChangesAsync(cancellationToken ?? CancellationToken.None);
    return Mapper.Map<BonTenant, BonTenantDto>(tenant);
  }

  public async Task<BonTenantDto> UpdateAsync(BonTenantId bonTenantId, BonTenantUpdateDto updateDto, CancellationToken? cancellationToken = default)
  {
    var tenant = await BonTenantRepository.FindByIdAsync(bonTenantId);
    if (tenant == null) throw new BonTenantNotFoundException(tenantId:bonTenantId,errorCode:$"{nameof(BonTenantBonApplicationService)}:{nameof(UpdateAsync)}");

    tenant.Key = updateDto.Key;
    await BonTenantRepository.UpdateAsync(tenant);

    return Mapper.Map<BonTenant, BonTenantDto>(tenant);
  }

  public async Task<BonTenantDto> DeleteAsync(BonTenantId bonTenantId, CancellationToken? cancellationToken = default)
  {
    var tenant = await BonTenantRepository.FindByIdAsync(bonTenantId);
    if (tenant == null) throw new BonTenantNotFoundException(tenantId:bonTenantId,errorCode:$"{nameof(BonTenantBonApplicationService)}:{nameof(DeleteAsync)}");

    await BonTenantRepository.DeleteAsync(tenant);

    return Mapper.Map<BonTenant, BonTenantDto>(tenant);
  }
}