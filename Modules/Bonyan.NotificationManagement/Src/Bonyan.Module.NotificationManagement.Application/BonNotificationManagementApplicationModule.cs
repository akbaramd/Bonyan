using System.Threading.Tasks;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Module.NotificationManagement.Abstractions;
using Bonyan.Module.NotificationManagement.Domain;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;
using Bonyan.UnitOfWork;

namespace Bonyan.Module.NotificationManagement.Application;

public class BonNotificationManagementApplicationModule : BonModule
{
    public BonNotificationManagementApplicationModule()
    {
        DependOn([
            typeof(BonNotificationManagementDomainModule),
            typeof(BonNotificationManagementAbstractionsModule),
            typeof(BonUnitOfWorkModule)
        ]);
    }


      
} 