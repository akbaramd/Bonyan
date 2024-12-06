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

public class BonIdentityManagementApplicationModule : BonModule 
{
    public BonIdentityManagementApplicationModule()
    {
        DependOn<BonUserManagementApplicationModule<BonIdentityUser>>();
        DependOn<BonIdentityManagementModule<BonIdentityUser>>();
        DependOn<BonWorkersModule>();
    }

    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddSingleton<BonIdentityPermissionSeeder>();
        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddTransient<IBonIdentityAuthAppService, BonIdentityAuthAppAppService<BonIdentityUser>>();
        context.Services.AddTransient<IBonIdentityPermissionAppService, BonIdentityPermissionAppService>();
        context.Services.AddTransient<IBonIdentityRoleAppService, BonIdentityRoleAppService>();
        context.Services.AddTransient<IBonIdentityUserAppService, BonIdentityUserAppService<BonIdentityUser>>();
        
        Configure<BonAutoMapperOptions>(c =>
        {
            c.AddProfile<BonIdentityUserMapper<BonIdentityUser>>();
            c.AddProfile<BonIdentityPermissionMapper>();
            c.AddProfile<BonIdentityRoleMapper>();
        });
        
        return base.OnConfigureAsync(context);
    }

    public override async Task OnPostInitializeAsync(BonInitializedContext context)
    {
        var seeder = context.GetRequireService<BonIdentityPermissionSeeder>();
        await seeder.StartAsync(CancellationToken.None);
        await base.OnPostInitializeAsync(context);
    }
}