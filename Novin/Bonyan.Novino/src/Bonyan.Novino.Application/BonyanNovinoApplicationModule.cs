using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Module.NotificationManagement.Abstractions.Providers;
using Bonyan.Module.NotificationManagement.Application;
using Bonyan.Module.NotificationManagement.Application.Providers;
using Bonyan.Novino.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Novino.Application;

public class BonyanNovinoApplicationModule : BonModule
{
    public BonyanNovinoApplicationModule()
    {
        DependOn<BonyanNovinoDomainModule>();
        DependOn<BonNotificationManagementApplicationModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddTransient<INotificationSender,NotificationSender>();
        context.Services.AddSingleton<NotificationProviderResolver>();
        return base.OnConfigureAsync(context);
    }
} 