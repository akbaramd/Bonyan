using Bonyan.AutoMapper;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.Application;

public class BonIdentityManagementApplicationModule<TUser> : BonModule where TUser : class, IBonIdentityUser
{
    public BonIdentityManagementApplicationModule()
    {
        DependOn<BonUserManagementApplicationModule<TUser>>();
        DependOn<BonIdentityManagementModule<TUser>>();
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