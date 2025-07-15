using Bonyan.IdentityManagement.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Novino.Web.Models;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.Novino.Domain;

public class BonyanNovinoDomainModule : BonModule
{
    public BonyanNovinoDomainModule()
    {
        DependOn<BonIdentityManagementDomainModule<Web.Models.User,Role>>();
        DependOn<BonTenantManagementDomainModule>();
    }
} 