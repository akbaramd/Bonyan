using Bonyan.IdentityManagement.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Module.NotificationManagement.Domain;
using Bonyan.Novino.Domain.Entities;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.Novino.Domain;

public class BonyanNovinoDomainModule : BonModule
{
    public BonyanNovinoDomainModule()
    {
        DependOn<BonIdentityManagementDomainModule<Entities.User,Role>>();
        DependOn<BonTenantManagementDomainModule>();
        DependOn<BonNotificationManagementDomainModule>();
    }
} 