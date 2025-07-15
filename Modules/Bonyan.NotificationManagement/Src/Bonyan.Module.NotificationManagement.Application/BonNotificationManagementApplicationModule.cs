using System.Threading.Tasks;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Module.NotificationManagement.Abstractions;
using Bonyan.Module.NotificationManagement.Abstractions.Providers;
using Bonyan.Module.NotificationManagement.Application.Providers;
using Bonyan.Module.NotificationManagement.Domain;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;
using Bonyan.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

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


    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddTransient<INotificationSender,NotificationSender>();
        context.Services.AddSingleton<NotificationProviderResolver>();
        
        return base.OnPreConfigureAsync(context);
    }
} 