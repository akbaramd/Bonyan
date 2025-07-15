using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Module.NotificationManagement.Domain;

public class BonNotificationManagementDomainModule : BonModule
{
    public BonNotificationManagementDomainModule()
    {
        DependOn([
            typeof(BonLayerDomainModule)
        ]);
    }

}