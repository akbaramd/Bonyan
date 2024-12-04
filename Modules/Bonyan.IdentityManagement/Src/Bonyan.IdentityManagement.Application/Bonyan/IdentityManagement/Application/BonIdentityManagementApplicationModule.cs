using Bonyan.AutoMapper;
using Bonyan.IdentityManagement.Application.Auth;
using Bonyan.IdentityManagement.Application.Permissions;
using Bonyan.IdentityManagement.Application.Permissions.Workers;
using Bonyan.IdentityManagement.Application.Roles;
using Bonyan.IdentityManagement.Application.Users;
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
        context.Services.AddTransient<IBonIdentityAuthAppService, BonIdentityAuthAppAppService<TUser>>();
        context.Services.AddTransient<IBonIdentityPermissionAppService, BonIdentityPermissionAppService>();
        context.Services.AddTransient<IBonIdentityRoleAppService, BonIdentityRoleAppService>();
        context.Services.AddTransient<IBonIdentityUserAppService, BonIdentityUserAppService<TUser>>();
        
        Configure<BonAutoMapperOptions>(c =>
        {
            c.AddProfile<BonIdentityUserMapper<TUser>>();
            c.AddProfile<BonIdentityPermissionMapper>();
            c.AddProfile<BonIdentityRoleMapper>();
        });
        
        return base.OnConfigureAsync(context);
    }
}