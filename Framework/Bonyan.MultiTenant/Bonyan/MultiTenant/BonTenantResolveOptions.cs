using JetBrains.Annotations;

namespace Bonyan.MultiTenant;

public class BonTenantResolveOptions
{
    [NotNull]
    public List<ITenantResolveContributor> TenantResolvers { get; }

    public BonTenantResolveOptions()
    {
        TenantResolvers = new List<ITenantResolveContributor>();
    }
}
