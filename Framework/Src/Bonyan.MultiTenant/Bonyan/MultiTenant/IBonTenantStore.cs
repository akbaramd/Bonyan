namespace Bonyan.MultiTenant;

public interface IBonTenantStore
{
    Task<BonTenantConfiguration?> FindAsync(string name);

    Task<BonTenantConfiguration?> FindAsync(Guid id);


}
