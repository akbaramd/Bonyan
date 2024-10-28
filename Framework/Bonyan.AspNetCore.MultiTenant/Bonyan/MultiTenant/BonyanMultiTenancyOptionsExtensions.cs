using Bonyan.AspNetCore.MultiTenant;

namespace Bonyan.MultiTenant;

public static class BonyanMultiTenancyOptionsExtensions
{
    public static void AddDomainTenantResolver(this BonyanTenantResolveOptions options, string domainFormat)
    {
        options.TenantResolvers.InsertAfter(
            r => r is CurrentUserTenantResolveContributor,
            new DomainTenantResolveContributor(domainFormat)
        );
    }
}
