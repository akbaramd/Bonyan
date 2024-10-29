using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Model;
using Bonyan.TenantManagement.Application.Dto;
using Bonyan.TenantManagement.Application.Exceptions;
using Bonyan.TenantManagement.Application.Specs;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.Application.Services;

public class TenantApplicationService : ApplicationService, ITenantApplicationService
{
  public ITenantRepository TenantRepository => ServiceProvider.LazyGetRequiredService<ITenantRepository>();

  public async Task<PaginatedResult<TenantDto>> PaginateAsync(TenantFilterDto paginateDto, CancellationToken? cancellationToken = default)
  {
    var paginated = await TenantRepository.PaginatedAsync(new TenantPaginateSpec(paginateDto));
    return Mapper.Map<PaginatedResult<Tenant>, PaginatedResult<TenantDto>>(paginated);
  }

  public async Task<TenantDto> DetailAsync(TenantId tenantId, CancellationToken? cancellationToken = default)
  {
    var tenant = await TenantRepository.FindByIdAsync(tenantId);
    if (tenant == null) throw new TenantNotFoundException(tenantId:tenantId,errorCode:$"{nameof(TenantApplicationService)}:{nameof(DetailAsync)}");

    return Mapper.Map<Tenant, TenantDto>(tenant);
  }

  public async Task<TenantDto> CreateAsync(TenantCreateDto createDto, CancellationToken? cancellationToken = default)
  {
    var tenant = new Tenant(createDto.Key);
    await TenantRepository.AddAsync(tenant);
    await UnitOfWork.SaveChangesAsync(cancellationToken ?? CancellationToken.None);

    return Mapper.Map<Tenant, TenantDto>(tenant);
  }

  public async Task<TenantDto> UpdateAsync(TenantId tenantId, TenantUpdateDto updateDto, CancellationToken? cancellationToken = default)
  {
    var tenant = await TenantRepository.FindByIdAsync(tenantId);
    if (tenant == null) throw new TenantNotFoundException(tenantId:tenantId,errorCode:$"{nameof(TenantApplicationService)}:{nameof(UpdateAsync)}");

    tenant.Key = updateDto.Key;
    await TenantRepository.UpdateAsync(tenant);
    await UnitOfWork.SaveChangesAsync();

    return Mapper.Map<Tenant, TenantDto>(tenant);
  }

  public async Task<TenantDto> DeleteAsync(TenantId tenantId, CancellationToken? cancellationToken = default)
  {
    var tenant = await TenantRepository.FindByIdAsync(tenantId);
    if (tenant == null) throw new TenantNotFoundException(tenantId:tenantId,errorCode:$"{nameof(TenantApplicationService)}:{nameof(DeleteAsync)}");

    await TenantRepository.DeleteAsync(tenant);
    await UnitOfWork.SaveChangesAsync(cancellationToken ?? CancellationToken.None);

    return Mapper.Map<Tenant, TenantDto>(tenant);
  }
}