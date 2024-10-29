using Bonyan.Exceptions;
using Bonyan.Layer.Domain.Services;

namespace Bonyan.TenantManagement.Domain;

public class TenantManager : DomainService, ITenantManager
{
  protected ITenantRepository TenantRepository { get; }

  public TenantManager(ITenantRepository tenantRepository)
  {
    TenantRepository = tenantRepository;

  }

  public virtual async Task<Tenant> CreateAsync(string key)
  {
    Check.NotNull(key, nameof(key));

    await ValidateNameAsync(key);
    return new Tenant( key);
  }

  public virtual async Task ChangeNameAsync(Tenant tenant, string key)
  {
    Check.NotNull(tenant, nameof(tenant));
    Check.NotNull(key, nameof(key));

    await ValidateNameAsync(key);
    tenant.SetKey(key);
  }

  protected virtual async Task ValidateNameAsync(string key, TenantId? expectedId = null)
  {
    var tenant = await TenantRepository.FindByKeyAsync(key);
    if (tenant != null && tenant.Id != expectedId)
    {
      throw new BusinessException("Bonyan:TenantManagement:DuplicateTenantName").WithData("Key", key);
    }
  }
}
