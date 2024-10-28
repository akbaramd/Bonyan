using Bonyan.Exceptions;

namespace Bonyan.MultiTenant;

public class TenantConfigurationProvider : ITenantConfigurationProvider
{
    protected virtual ITenantResolver TenantResolver { get; }
    protected virtual ITenantStore TenantStore { get; }
    protected virtual ITenantResolveResultAccessor TenantResolveResultAccessor { get; }


    public TenantConfigurationProvider(
        ITenantResolver tenantResolver,
        ITenantStore tenantStore,
        ITenantResolveResultAccessor tenantResolveResultAccessor)
    {
        TenantResolver = tenantResolver;
        TenantStore = tenantStore;
        TenantResolveResultAccessor = tenantResolveResultAccessor;
    }

    public virtual async Task<TenantConfiguration?> GetAsync(bool saveResolveResult = false)
    {
        var resolveResult = await TenantResolver.ResolveTenantIdOrNameAsync();

        if (saveResolveResult)
        {
            TenantResolveResultAccessor.Result = resolveResult;
        }

        TenantConfiguration? tenant = null;
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

    protected virtual async Task<TenantConfiguration?> FindTenantAsync(string tenantIdOrName)
    {
        if (Guid.TryParse(tenantIdOrName, out var parsedTenantId))
        {
            return await TenantStore.FindAsync(parsedTenantId);
        }
        else
        {
            return await TenantStore.FindAsync(tenantIdOrName);
        }
    }
}
