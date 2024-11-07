using Bonyan.Exceptions;

namespace Bonyan.MultiTenant;

public class TenantConfigurationProvider : ITenantConfigurationProvider
{
    protected virtual ITenantResolver TenantResolver { get; }
    protected virtual IBonTenantStore BonTenantStore { get; }
    protected virtual ITenantResolveResultAccessor TenantResolveResultAccessor { get; }


    public TenantConfigurationProvider(
        ITenantResolver tenantResolver,
        IBonTenantStore bonTenantStore,
        ITenantResolveResultAccessor tenantResolveResultAccessor)
    {
        TenantResolver = tenantResolver;
        BonTenantStore = bonTenantStore;
        TenantResolveResultAccessor = tenantResolveResultAccessor;
    }

    public virtual async Task<BonTenantConfiguration?> GetAsync(bool saveResolveResult = false)
    {
        var resolveResult = await TenantResolver.ResolveTenantIdOrNameAsync();

        if (saveResolveResult)
        {
            TenantResolveResultAccessor.Result = resolveResult;
        }

        BonTenantConfiguration? tenant = null;
        if (resolveResult.TenantIdOrName != null)
        {
            tenant = await FindTenantAsync(resolveResult.TenantIdOrName);

            if (tenant == null)
            {
                throw new BusinessException(
                    code: "Volo.BonyanIo.MultiTenancy:010001",
                    message: "TenantNotFoundMessage",
                    details: "TenantNotFoundDetails"
                );
            }

            if (!tenant.IsActive)
            {
                throw new BusinessException(
                    code: "Volo.BonyanIo.MultiTenancy:010002",
                    message:"TenantNotActiveMessage",
                    details: "TenantNotActiveDetails"
                );
            }
        }

        return tenant;
    }

    protected virtual async Task<BonTenantConfiguration?> FindTenantAsync(string tenantIdOrName)
    {
        if (Guid.TryParse(tenantIdOrName, out var parsedTenantId))
        {
            return await BonTenantStore.FindAsync(parsedTenantId);
        }
        else
        {
            return await BonTenantStore.FindAsync(tenantIdOrName);
        }
    }
}
