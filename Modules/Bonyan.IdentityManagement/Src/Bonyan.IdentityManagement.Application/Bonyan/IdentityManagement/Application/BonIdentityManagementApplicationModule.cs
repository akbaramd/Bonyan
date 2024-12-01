using Bonyan.AutoMapper;
using Bonyan.IdentityManagement.Application.Workers;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Application;
using Bonyan.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.Application;

public class BonIdentityManagementApplicationModule<TUser> : BonModule where TUser : class, IBonIdentityUser
{
    public BonIdentityManagementApplicationModule()
    {
        DependOn<BonUserManagementApplicationModule<TUser>>();
        DependOn<BonIdentityManagementModule<TUser>>();
        DependOn<BonWorkersModule>();
    }

    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        PreConfigure<BonWorkerConfiguration>(c =>
        {
            c.RegisterWorker<BonIdentityPermissionSeeder>();
        });
        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddTransient<IBonIdentityAuthService, BonIdentityAuthService<TUser>>();
        
        Configure<BonAutoMapperOptions>(c =>
        {
            c.AddProfile<BonUserMapper<TUser>>();
        });
        
        return base.OnConfigureAsync(context);
    }
}