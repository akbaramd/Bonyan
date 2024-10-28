namespace Bonyan.MultiTenant;

public interface ITenantStore
{
    Task<TenantConfiguration?> FindAsync(string name);

    Task<TenantConfiguration?> FindAsync(Guid id);


}
