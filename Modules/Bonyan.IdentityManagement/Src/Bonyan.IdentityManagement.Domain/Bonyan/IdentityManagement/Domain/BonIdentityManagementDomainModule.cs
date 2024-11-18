using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.DomainServices;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.Domain;

public class BonIdentityManagementDomainModule<TUser> : Modularity.Abstractions.BonModule where TUser : class, IBonIdentityUser
{
    public BonIdentityManagementDomainModule()
    {
        DependOn([
            typeof(BonUserManagementDomainModule<TUser>),
        ]);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddTransient<BonIdentityRoleManager>();
        context.Services.AddTransient<BonIdentityUserManager<TUser>>();
        context.Services.AddTransient<IBonIdentityRoleManager>(s=>s.GetRequiredService<BonIdentityRoleManager>());
        context.Services.AddTransient<IBonIdentityUserManager<TUser>>(s=>s.GetRequiredService<BonIdentityUserManager<TUser>>());
   
        return base.OnConfigureAsync(context);
    }
}