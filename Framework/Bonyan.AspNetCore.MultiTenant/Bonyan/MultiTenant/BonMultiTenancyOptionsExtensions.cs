using Bonyan.AspNetCore.MultiTenant;

namespace Bonyan.MultiTenant;

public static class BonMultiTenancyOptionsExtensions
{
    public static void AddDomainTenantResolver(this BonTenantResolveOptions options, string domainFormat)
    {
        options.TenantResolvers.InsertAfter(
            r => r is CurrentUserTenantResolveContributor,
            new DomainTenantResolveContributor(domainFormat)
        );
    }
}
