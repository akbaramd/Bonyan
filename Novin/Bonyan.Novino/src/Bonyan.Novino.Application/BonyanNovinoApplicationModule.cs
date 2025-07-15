using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Module.NotificationManagement.Application;
using Bonyan.Novino.Domain;

namespace Bonyan.Novino.Application;

public class BonyanNovinoApplicationModule : BonModule
{
    public BonyanNovinoApplicationModule()
    {
        DependOn<BonyanNovinoDomainModule>();
        DependOn<BonNotificationManagementApplicationModule>();
    }
} 