using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Module.NotificationManagement.Domain;
using Bonyan.Module.NotificationManagement.Domain.Notifications.Repositories;
using Bonyan.Module.NotificationManagement.EntityFrameworkCore.Repositories;
using Microsoft.Extensions.DependencyInjection;


public class BonNotificationManagementEntityFrameworkCoreModule : BonModule
{
    public BonNotificationManagementEntityFrameworkCoreModule()
    {
        DependOn([
            typeof(BonNotificationManagementDomainModule),
            typeof(BonEntityFrameworkModule),
        ]);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.AddDbContext<BonNotificationManagementDbContext>();

        context.Services
            .AddTransient<IBonNotificationRepository, BonEfCoreNotificationRepository>();
        context.Services
            .AddTransient<IBonNotificationReadOnlyRepository,BonEfCoreNotificationReadOnlyRepository>();

        return base.OnConfigureAsync(context);
    }
}