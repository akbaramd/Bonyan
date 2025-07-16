using Bonyan.IdentityManagement.Application;
using Bonyan.IdentityManagement.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Module.NotificationManagement.Abstractions.Providers;
using Bonyan.Module.NotificationManagement.Application;
using Bonyan.Module.NotificationManagement.Application.Providers;
using Bonyan.Novino.Domain;
using Bonyan.Novino.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Novino.Application;

public class BonNovinoApplicationModule : BonModule
{
    public BonNovinoApplicationModule()
    {
        DependOn<BonyanNovinoDomainModule>();
        DependOn<BonIdentityManagementApplicationModule<Domain.Entities.User,Role>>();
        DependOn<BonNotificationManagementApplicationModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        return base.OnConfigureAsync(context);
    }
} 