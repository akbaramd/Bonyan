using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.Localization;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Novino.Application;
using Bonyan.Novino.Domain;
using Bonyan.Novino.Domain.Entities;
using Bonyan.Novino.Infrastructure.Data;
using Bonyan.TenantManagement.EntityFrameworkCore;
using Bonyan.VirtualFileSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Bonyan.Module.NotificationManagement.Abstractions.Providers;
using Bonyan.Module.NotificationManagement.Abstractions.Types;
using Bonyan.Novino.Infrastructure.Providers.Notifications;

namespace Bonyan.Novino.Infrastructure;


public class BonNovinoInfrastructureModule : BonModule
{
    public BonNovinoInfrastructureModule()
    {
        DependOn<BonyanNovinoDomainModule>();
        DependOn<BonIdentityManagementEntityFrameworkCoreModule<Domain.Entities.User,Role>>();
        DependOn<BonTenantManagementEntityFrameworkModule>();
        DependOn<BonNotificationManagementEntityFrameworkCoreModule>();
    }

    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddSingleton<INotificationProvider,InAppNotificationProviders>();

        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
       
        
        // Configure database context
        context.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = "Server=localhost\\AHMADI,1433;Database=BonyanNovino;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
            options.UseSqlServer(connectionString);
        });


       
    
        return base.OnConfigureAsync(context);
    }

    public override Task OnPostConfigureAsync(BonConfigurationContext context)
    {
        Configure<BonLocalizationOptions>(c =>
        {
            c.Languages.Clear();
            c.Languages.Add(new LanguageInfo("fa", "فارسی"));
        });
        return base.OnPostConfigureAsync(context);
    }
} 