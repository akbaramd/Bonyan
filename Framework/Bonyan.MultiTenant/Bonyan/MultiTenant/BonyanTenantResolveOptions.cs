using JetBrains.Annotations;

namespace Bonyan.MultiTenant;

public class BonyanTenantResolveOptions
{
    [NotNull]
    public List<ITenantResolveContributor> TenantResolvers { get; }

    public BonyanTenantResolveOptions()
    {
        TenantResolvers = new List<ITenantResolveContributor>();
    }
}
